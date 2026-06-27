using TaskManagement.DTO.Auth;

namespace TaskManagement.Services.Interfaces;

public interface IAuthService
{
    Task<LoginResponse> RegisterAsync(RegisterRequest request);
    Task<LoginResponse> LoginAsync(LoginRequest request, string? ipAddress = null);
    Task<LoginResponse> RefreshTokenAsync(string refreshToken);
    Task LogoutAsync(string refreshToken);
}
