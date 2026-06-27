using TaskManagement.Common;
using TaskManagement.Models;

namespace TaskManagement.Repositories.Interfaces;

public interface ITaskRepository
{
    Task<TaskItem?> GetByIdAsync(Guid id);
    Task<PagedResult<TaskItem>> GetAllAsync(int pageNumber, int pageSize, Guid? projectId = null, Models.TaskStatus? status = null, TaskPriority? priority = null, string? keyword = null);
    Task<IEnumerable<TaskItem>> GetByProjectIdAsync(Guid projectId);
    Task<IEnumerable<TaskItem>> GetByAssigneeIdAsync(Guid assigneeId);
    Task<TaskItem> CreateAsync(TaskItem task);
    Task<TaskItem> UpdateAsync(TaskItem task);
    Task DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
}
