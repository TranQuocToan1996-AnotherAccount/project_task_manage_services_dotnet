using TaskManagement.Common;
using TaskManagement.Models;

namespace TaskManagement.Services.Interfaces;

public interface IProjectService
{
    Task<Project?> GetProjectByIdAsync(Guid id);
    Task<PagedResult<Project>> GetAllProjectsAsync(int pageNumber, int pageSize);
    Task<IEnumerable<Project>> GetProjectsByOwnerIdAsync(Guid ownerId);
    Task<Project> CreateProjectAsync(Project project);
    Task<Project> UpdateProjectAsync(Guid id, Project project);
    Task DeleteProjectAsync(Guid id);
}
