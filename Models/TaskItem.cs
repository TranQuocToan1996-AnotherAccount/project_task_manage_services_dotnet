using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManagement.Models;

public enum TaskStatus
{
    Todo,
    InProgress,
    Done
}

public enum TaskPriority
{
    Low,
    Medium,
    High
}

[Table("tasks")]
public class TaskItem
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [MaxLength(200)]
    [Column("title")]
    public string Title { get; set; } = string.Empty;

    [MaxLength(1000)]
    [Column("description")]
    public string? Description { get; set; }

    [Required]
    [Column("status")]
    public TaskStatus Status { get; set; } = TaskStatus.Todo;

    [Required]
    [Column("priority")]
    public TaskPriority Priority { get; set; } = TaskPriority.Medium;

    [Required]
    [Column("project_id")]
    public Guid ProjectId { get; set; }

    [Column("assignee_id")]
    public Guid? AssigneeId { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    [ForeignKey(nameof(ProjectId))]
    public virtual Project Project { get; set; } = null!;
    
    [ForeignKey(nameof(AssigneeId))]
    public virtual User? Assignee { get; set; }
}
