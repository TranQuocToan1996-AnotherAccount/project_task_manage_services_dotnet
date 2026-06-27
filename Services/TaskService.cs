using TaskManagement.Common;
using TaskManagement.Constants;
using TaskManagement.Models;
using TaskManagement.Repositories.Interfaces;
using TaskManagement.Services.Interfaces;
using TaskManagement.Utility;

namespace TaskManagement.Services;

public class TaskService : ITaskService
{
    private readonly ITaskRepository _taskRepository;
    private readonly IProjectRepository _projectRepository;
    private readonly IUserRepository _userRepository;
    private readonly IRedisHelper _redisHelper;

    public TaskService(ITaskRepository taskRepository, IProjectRepository projectRepository, IUserRepository userRepository, IRedisHelper redisHelper)
    {
        _taskRepository = taskRepository;
        _projectRepository = projectRepository;
        _userRepository = userRepository;
        _redisHelper = redisHelper;
    }

    public async Task<TaskItem?> GetTaskByIdAsync(Guid id)
    {
        var task = await _taskRepository.GetByIdAsync(id);

        if (task == null)
            throw new KeyNotFoundException($"Task with id {id} not found");
            
        return task;
    }

    public async Task<PagedResult<TaskItem>> GetAllTasksAsync(int pageNumber, int pageSize, Guid? projectId = null, Models.TaskStatus? status = null, TaskPriority? priority = null, string? keyword = null)
    {
        return await _taskRepository.GetAllAsync(pageNumber, pageSize, projectId, status, priority, keyword);
    }

    public async Task<TaskItem> CreateTaskAsync(TaskItem task)
    {
        var projectExists = await _projectRepository.GetByIdAsync(task.ProjectId);
        if (projectExists == null)
            throw new KeyNotFoundException($"Project with id {task.ProjectId} not found");

        if (!task.AssigneeId.HasValue)
            throw new InvalidOperationException("AssigneeId is required for creating a task");


        var assigneeExists = await _userRepository.ExistsAsync(task.AssigneeId.Value);
        if (!assigneeExists)
            throw new KeyNotFoundException($"Assignee with id {task.AssigneeId} not found");

        return await _taskRepository.CreateAsync(task);
    }

    public async Task<TaskItem> UpdateTaskAsync(Guid id, TaskItem task)
    {
        var existingTask = await _taskRepository.GetByIdAsync(id);
        if (existingTask == null)
            throw new KeyNotFoundException($"Task with id {id} not found");

        existingTask.Title = task.Title;
        existingTask.Description = task.Description;
        existingTask.Status = task.Status;
        existingTask.Priority = task.Priority;

        if (task.AssigneeId.HasValue)
        {
            var assigneeExists = await _userRepository.ExistsAsync(task.AssigneeId.Value);
            if (!assigneeExists)
                throw new KeyNotFoundException($"Assignee with id {task.AssigneeId} not found");
            existingTask.AssigneeId = task.AssigneeId;
        }

        var updatedTask = await _taskRepository.UpdateAsync(existingTask);
        
        // Invalidate cache
        var cacheKey = CacheKeys.Task(id);
        await _redisHelper.RemoveAsync(cacheKey);
        
        return updatedTask;
    }

    public async Task DeleteTaskAsync(Guid id)
    {
        var exists = await _taskRepository.ExistsAsync(id);
        if (!exists)
            throw new KeyNotFoundException($"Task with id {id} not found");

        await _taskRepository.DeleteAsync(id);
        
        // Invalidate cache
        var cacheKey = CacheKeys.Task(id);
        await _redisHelper.RemoveAsync(cacheKey);
    }

    public async Task<TaskItem> AssignTaskAsync(Guid taskId, Guid assigneeId)
    {
        var task = await _taskRepository.GetByIdAsync(taskId);
        if (task == null)
            throw new KeyNotFoundException($"Task with id {taskId} not found");

        var assigneeExists = await _userRepository.ExistsAsync(assigneeId);
        if (!assigneeExists)
            throw new KeyNotFoundException($"Assignee with id {assigneeId} not found");

        task.AssigneeId = assigneeId;
        return await _taskRepository.UpdateAsync(task);
    }

    public async Task<TaskItem> UpdateTaskStatusAsync(Guid id, Models.TaskStatus status)
    {
        var task = await _taskRepository.GetByIdAsync(id);
        if (task == null)
            throw new KeyNotFoundException($"Task with id {id} not found");

        task.Status = status;
        return await _taskRepository.UpdateAsync(task);
    }
}
