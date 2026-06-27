using System.ComponentModel.DataAnnotations;
using TaskManagement.Models;

namespace TaskManagement.DTO.Task;

public class CreateTaskRequest
{
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Description { get; set; }

    [Required]
    public Guid ProjectId { get; set; }

    public Models.TaskStatus Status { get; set; } = Models.TaskStatus.Todo;

    public TaskPriority Priority { get; set; } = TaskPriority.Medium;

    public Guid? AssigneeId { get; set; }
}
