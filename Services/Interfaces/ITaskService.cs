using TaskManagement.Common;
using TaskManagement.Models;

namespace TaskManagement.Services.Interfaces;

public interface ITaskService
{
    Task<TaskItem?> GetTaskByIdAsync(Guid id);
    Task<PagedResult<TaskItem>> GetAllTasksAsync(int pageNumber, int pageSize, Guid? projectId = null, Models.TaskStatus? status = null, TaskPriority? priority = null, string? keyword = null);
    Task<TaskItem> CreateTaskAsync(TaskItem task);
    Task<TaskItem> UpdateTaskAsync(Guid id, TaskItem task);
    Task DeleteTaskAsync(Guid id);
    Task<TaskItem> AssignTaskAsync(Guid taskId, Guid assigneeId);
    Task<TaskItem> UpdateTaskStatusAsync(Guid id, Models.TaskStatus status);
}
