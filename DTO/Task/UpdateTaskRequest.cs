using System.ComponentModel.DataAnnotations;
using TaskManagement.Models;

namespace TaskManagement.DTO.Task;

public class UpdateTaskRequest
{
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Description { get; set; }

    public Models.TaskStatus Status { get; set; }

    public TaskPriority Priority { get; set; }

    public Guid? AssigneeId { get; set; }
}
