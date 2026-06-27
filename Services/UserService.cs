using TaskManagement.Models;
using TaskManagement.Repositories.Interfaces;
using TaskManagement.Services.Interfaces;

namespace TaskManagement.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<User?> GetUserByIdAsync(Guid id)
    {
        return await _userRepository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        return await _userRepository.GetAllAsync();
    }

    public async Task<User> CreateUserAsync(User user)
    {
        var existingUser = await _userRepository.GetByEmailAsync(user.Email);
        if (existingUser != null)
            throw new InvalidOperationException("User with this email already exists");

        existingUser = await _userRepository.GetByUsernameAsync(user.Username);
        if (existingUser != null)
            throw new InvalidOperationException("User with this username already exists");

        return await _userRepository.CreateAsync(user);
    }

    public async Task<User> UpdateUserAsync(Guid id, User user)
    {
        var existingUser = await _userRepository.GetByIdAsync(id);
        if (existingUser == null)
            throw new KeyNotFoundException($"User with id {id} not found");

        existingUser.Username = user.Username;
        existingUser.Email = user.Email;
        if (!string.IsNullOrEmpty(user.PasswordHash))
            existingUser.PasswordHash = user.PasswordHash;

        return await _userRepository.UpdateAsync(existingUser);
    }

    public async Task DeleteUserAsync(Guid id)
    {
        var exists = await _userRepository.ExistsAsync(id);
        if (!exists)
            throw new KeyNotFoundException($"User with id {id} not found");

        await _userRepository.DeleteAsync(id);
    }
}
