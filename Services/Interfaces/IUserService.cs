using TaskManagement.Models;

namespace TaskManagement.Services.Interfaces;

public interface IUserService
{
    Task<User?> GetUserByIdAsync(Guid id);
    Task<IEnumerable<User>> GetAllUsersAsync();
    Task<User> CreateUserAsync(User user);
    Task<User> UpdateUserAsync(Guid id, User user);
    Task DeleteUserAsync(Guid id);
}
