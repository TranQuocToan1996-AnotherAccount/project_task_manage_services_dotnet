using System.ComponentModel.DataAnnotations;

namespace TaskManagement.DTO.Auth;

public class RefreshTokenRequest
{
    [Required]
    public required string RefreshToken { get; set; }
}
