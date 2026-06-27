using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.Common;
using TaskManagement.DTO.User;
using TaskManagement.Models;
using TaskManagement.Services.Interfaces;

namespace TaskManagement.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<UserResponse>>>> GetAllUsers() // TODO: Add pagination because call like this will affect Database performance if there are too many users
    {
        try
        {
            var users = await _userService.GetAllUsersAsync();
            var userResponses = users.Select(u => new UserResponse
            {
                Id = u.Id,
                Username = u.Username,
                Email = u.Email,
                CreatedAt = u.CreatedAt,
                UpdatedAt = u.UpdatedAt
            });
            return Ok(ApiResponse<IEnumerable<UserResponse>>.Ok(userResponses));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<IEnumerable<UserResponse>>.Fail(ex.Message));
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<UserResponse>>> GetUserById(Guid id)
    {
        try
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
                return NotFound(ApiResponse<UserResponse>.Fail("User not found"));

            var userResponse = new UserResponse
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            };
            return Ok(ApiResponse<UserResponse>.Ok(userResponse));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<UserResponse>.Fail(ex.Message));
        }
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<UserResponse>>> CreateUser([FromBody] CreateUserRequest request)
    {
        try
        {
            var user = new User
            {
                Username = request.Username,
                Email = request.Email,
                PasswordHash = request.Password // This should be hashed in real implementation
            };
            var createdUser = await _userService.CreateUserAsync(user);
            
            var userResponse = new UserResponse
            {
                Id = createdUser.Id,
                Username = createdUser.Username,
                Email = createdUser.Email,
                CreatedAt = createdUser.CreatedAt,
                UpdatedAt = createdUser.UpdatedAt
            };
            return CreatedAtAction(nameof(GetUserById), new { id = createdUser.Id }, ApiResponse<UserResponse>.Ok(userResponse, "User created successfully"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<UserResponse>.Fail(ex.Message));
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<UserResponse>>> UpdateUser(Guid id, [FromBody] UpdateUserRequest request)
    {
        try
        {
            var user = new User
            {
                Username = request.Username,
                Email = request.Email,
                PasswordHash = request.Password ?? string.Empty
            };
            var updatedUser = await _userService.UpdateUserAsync(id, user);
            
            var userResponse = new UserResponse
            {
                Id = updatedUser.Id,
                Username = updatedUser.Username,
                Email = updatedUser.Email,
                CreatedAt = updatedUser.CreatedAt,
                UpdatedAt = updatedUser.UpdatedAt
            };
            return Ok(ApiResponse<UserResponse>.Ok(userResponse, "User updated successfully"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<UserResponse>.Fail(ex.Message));
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteUser(Guid id)
    {
        try
        {
            await _userService.DeleteUserAsync(id);
            return Ok(ApiResponse<object>.Ok(null, "User deleted successfully"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<object>.Fail(ex.Message));
        }
    }
}
