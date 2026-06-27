using Microsoft.Extensions.Options;
using TaskManagement.Configurations;
using TaskManagement.DTO.Auth;
using TaskManagement.DTO.Kafka;
using TaskManagement.Models;
using TaskManagement.Repositories.Interfaces;
using TaskManagement.Services.Interfaces;
using TaskManagement.Utility;

namespace TaskManagement.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly JwtOptions _jwtOptions;
    private readonly IKafkaProducer _kafkaProducer;
    private static readonly Dictionary<string, Guid> _refreshTokens = new();

    public AuthService(IUserRepository userRepository, IOptions<JwtOptions> jwtOptions, IKafkaProducer kafkaProducer)
    {
        _userRepository = userRepository;
        _jwtOptions = jwtOptions.Value;
        _kafkaProducer = kafkaProducer;
    }

    public async Task<LoginResponse> RegisterAsync(RegisterRequest request)
    {
        var existingUser = await _userRepository.GetByEmailAsync(request.Email);
        if (existingUser != null)
            throw new InvalidOperationException("User with this email already exists");

        existingUser = await _userRepository.GetByUsernameAsync(request.Username);
        if (existingUser != null)
            throw new InvalidOperationException("User with this username already exists");

        var user = new User
        {
            Username = request.Username,
            Email = request.Email,
            PasswordHash = PasswordHasher.HashPassword(request.Password)
        };

        var createdUser = await _userRepository.CreateAsync(user);

        var accessToken = JwtHelper.GenerateToken(createdUser.Id, createdUser.Username, _jwtOptions);
        var refreshToken = JwtHelper.GenerateRefreshToken();
        _refreshTokens[refreshToken] = createdUser.Id;

        return new LoginResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtOptions.ExpirationInMinutes)
        };
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request, string? ipAddress = null)
    {
        var user = await _userRepository.GetByUsernameAsync(request.Username);
        if (user == null)
            throw new UnauthorizedAccessException("Invalid username or password");

        if (!PasswordHasher.VerifyPassword(request.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid username or password");

        var accessToken = JwtHelper.GenerateToken(user.Id, user.Username, _jwtOptions);
        var refreshToken = JwtHelper.GenerateRefreshToken();
        _refreshTokens[refreshToken] = user.Id;

        // Publish login event to Kafka (fire-and-forget)
        var loginEvent = new LoginEvent
        {
            UserId = user.Id,
            Timestamp = DateTime.UtcNow,
            IpAddress = ipAddress ?? "unknown"
        };
        _ = Task.Run(() => _kafkaProducer.PublishLoginEventAsync(loginEvent));

        return new LoginResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtOptions.ExpirationInMinutes)
        };
    }

    public async Task<LoginResponse> RefreshTokenAsync(string refreshToken)
    {
        if (!_refreshTokens.ContainsKey(refreshToken))
            throw new UnauthorizedAccessException("Invalid refresh token");

        var userId = _refreshTokens[refreshToken];
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
            throw new UnauthorizedAccessException("User not found");

        _refreshTokens.Remove(refreshToken);

        var accessToken = JwtHelper.GenerateToken(user.Id, user.Username, _jwtOptions);
        var newRefreshToken = JwtHelper.GenerateRefreshToken();
        _refreshTokens[newRefreshToken] = user.Id;

        return new LoginResponse
        {
            AccessToken = accessToken,
            RefreshToken = newRefreshToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtOptions.ExpirationInMinutes)
        };
    }

    public async Task LogoutAsync(string refreshToken)
    {
        if (_refreshTokens.ContainsKey(refreshToken))
        {
            _refreshTokens.Remove(refreshToken);
        }
        await Task.CompletedTask;
    }
}
