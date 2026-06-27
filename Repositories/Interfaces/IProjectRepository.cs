using TaskManagement.Common;
using TaskManagement.Models;

namespace TaskManagement.Repositories.Interfaces;

public interface IProjectRepository
{
    Task<Project?> GetByIdAsync(Guid id);
    Task<PagedResult<Project>> GetAllAsync(int pageNumber, int pageSize);
    Task<IEnumerable<Project>> GetByOwnerIdAsync(Guid ownerId);
    Task<Project> CreateAsync(Project project);
    Task<Project> UpdateAsync(Project project);
    Task DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
}
