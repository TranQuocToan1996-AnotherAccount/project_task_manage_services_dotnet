using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.Common;
using TaskManagement.DTO.Task;
using TaskManagement.Models;
using TaskManagement.Services.Interfaces;

namespace TaskManagement.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TaskController : ControllerBase
{
    private readonly ITaskService _taskService;

    public TaskController(ITaskService taskService)
    {
        _taskService = taskService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<TaskResponse>>>> GetAllTasks(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] Guid? projectId = null,
        [FromQuery] Models.TaskStatus? status = null,
        [FromQuery] TaskPriority? priority = null,
        [FromQuery] string? keyword = null)
    {
        try
        {
            var tasks = await _taskService.GetAllTasksAsync(pageNumber, pageSize, projectId, status, priority, keyword);
            var taskResponses = tasks.Items.Select(t => new TaskResponse
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                Status = t.Status,
                Priority = t.Priority,
                ProjectId = t.ProjectId,
                ProjectName = t.Project?.Name ?? string.Empty,
                AssigneeId = t.AssigneeId,
                AssigneeName = t.Assignee?.Username,
                CreatedAt = t.CreatedAt,
                UpdatedAt = t.UpdatedAt
            }).ToList();
            
            var pagedResult = new PagedResult<TaskResponse>
            {
                Items = taskResponses,
                Pagination = tasks.Pagination
            };
            return Ok(ApiResponse<PagedResult<TaskResponse>>.Ok(pagedResult));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<PagedResult<TaskResponse>>.Fail(ex.Message));
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<TaskResponse>>> GetTaskById(Guid id)
    {
        try
        {
            var task = await _taskService.GetTaskByIdAsync(id);
            if (task == null)
                return NotFound(ApiResponse<TaskResponse>.Fail("Task not found"));

            var taskResponse = new TaskResponse
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                Status = task.Status,
                Priority = task.Priority,
                ProjectId = task.ProjectId,
                ProjectName = task.Project?.Name ?? string.Empty,
                AssigneeId = task.AssigneeId,
                AssigneeName = task.Assignee?.Username,
                CreatedAt = task.CreatedAt,
                UpdatedAt = task.UpdatedAt
            };
            return Ok(ApiResponse<TaskResponse>.Ok(taskResponse));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<TaskResponse>.Fail(ex.Message));
        }
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<TaskResponse>>> CreateTask([FromBody] CreateTaskRequest request)
    {
        try
        {
            var task = new TaskItem
            {
                Title = request.Title,
                Description = request.Description,
                ProjectId = request.ProjectId,
                Status = request.Status,
                Priority = request.Priority,
                AssigneeId = request.AssigneeId
            };
            var createdTask = await _taskService.CreateTaskAsync(task);
            
            var taskResponse = new TaskResponse
            {
                Id = createdTask.Id,
                Title = createdTask.Title,
                Description = createdTask.Description,
                Status = createdTask.Status,
                Priority = createdTask.Priority,
                ProjectId = createdTask.ProjectId,
                ProjectName = createdTask.Project?.Name ?? string.Empty,
                AssigneeId = createdTask.AssigneeId,
                AssigneeName = createdTask.Assignee?.Username,
                CreatedAt = createdTask.CreatedAt,
                UpdatedAt = createdTask.UpdatedAt
            };
            return CreatedAtAction(nameof(GetTaskById), new { id = createdTask.Id }, ApiResponse<TaskResponse>.Ok(taskResponse, "Task created successfully"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<TaskResponse>.Fail(ex.Message));
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<TaskResponse>>> UpdateTask(Guid id, [FromBody] UpdateTaskRequest request)
    {
        try
        {
            var task = new TaskItem
            {
                Title = request.Title,
                Description = request.Description,
                Status = request.Status,
                Priority = request.Priority,
                AssigneeId = request.AssigneeId
            };
            var updatedTask = await _taskService.UpdateTaskAsync(id, task);
            
            var taskResponse = new TaskResponse
            {
                Id = updatedTask.Id,
                Title = updatedTask.Title,
                Description = updatedTask.Description,
                Status = updatedTask.Status,
                Priority = updatedTask.Priority,
                ProjectId = updatedTask.ProjectId,
                ProjectName = updatedTask.Project?.Name ?? string.Empty,
                AssigneeId = updatedTask.AssigneeId,
                AssigneeName = updatedTask.Assignee?.Username, // TODO: Bug null, I keep it as a challenge for you to fix it
                CreatedAt = updatedTask.CreatedAt,
                UpdatedAt = updatedTask.UpdatedAt
            };
            return Ok(ApiResponse<TaskResponse>.Ok(taskResponse, "Task updated successfully"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<TaskResponse>.Fail(ex.Message));
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteTask(Guid id)
    {
        try
        {
            await _taskService.DeleteTaskAsync(id);
            return Ok(ApiResponse<object>.Ok(null, "Task deleted successfully"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<object>.Fail(ex.Message));
        }
    }

    [HttpPost("{id}/assign")]
    public async Task<ActionResult<ApiResponse<TaskResponse>>> AssignTask(Guid id, [FromBody] Guid assigneeId)
    {
        try
        {
            var task = await _taskService.AssignTaskAsync(id, assigneeId);
            
            var taskResponse = new TaskResponse
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                Status = task.Status,
                Priority = task.Priority,
                ProjectId = task.ProjectId,
                ProjectName = task.Project?.Name ?? string.Empty,
                AssigneeId = task.AssigneeId,
                AssigneeName = task.Assignee?.Username,
                CreatedAt = task.CreatedAt,
                UpdatedAt = task.UpdatedAt
            };
            return Ok(ApiResponse<TaskResponse>.Ok(taskResponse, "Task assigned successfully"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<TaskResponse>.Fail(ex.Message));
        }
    }

    [HttpPatch("{id}/status")]
    public async Task<ActionResult<ApiResponse<TaskResponse>>> UpdateTaskStatus(Guid id, [FromBody] Models.TaskStatus status)
    {
        try
        {
            var task = await _taskService.UpdateTaskStatusAsync(id, status);
            
            var taskResponse = new TaskResponse
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                Status = task.Status,
                Priority = task.Priority,
                ProjectId = task.ProjectId,
                ProjectName = task.Project?.Name ?? string.Empty,
                AssigneeId = task.AssigneeId,
                AssigneeName = task.Assignee?.Username,
                CreatedAt = task.CreatedAt,
                UpdatedAt = task.UpdatedAt
            };
            return Ok(ApiResponse<TaskResponse>.Ok(taskResponse, "Task status updated successfully"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<TaskResponse>.Fail(ex.Message));
        }
    }
}
