using System.ComponentModel.DataAnnotations;

namespace TaskManagement.DTO.Auth;

public class LogoutRequest
{
    [Required]
    public required string RefreshToken { get; set; }
}
