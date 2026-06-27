using System.ComponentModel.DataAnnotations;

namespace TaskManagement.DTO.User;

public class UpdateUserRequest
{
    [Required]
    [MinLength(3)]
    public string Username { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    public string? Password { get; set; }
}
