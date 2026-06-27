using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.Common;
using TaskManagement.DTO.Project;
using TaskManagement.Models;
using TaskManagement.Services.Interfaces;

namespace TaskManagement.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProjectController : ControllerBase
{
    private readonly IProjectService _projectService;

    public ProjectController(IProjectService projectService)
    {
        _projectService = projectService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<ProjectResponse>>>> GetAllProjects([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        try
        {
            var projects = await _projectService.GetAllProjectsAsync(pageNumber, pageSize);
            var projectResponses = projects.Items.Select(p => new ProjectResponse
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                OwnerId = p.OwnerId,
                OwnerName = p.Owner?.Username ?? string.Empty,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt
            }).ToList();
            
            var pagedResult = new PagedResult<ProjectResponse>
            {
                Items = projectResponses,
                Pagination = projects.Pagination
            };
            return Ok(ApiResponse<PagedResult<ProjectResponse>>.Ok(pagedResult));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<PagedResult<ProjectResponse>>.Fail(ex.Message));
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<ProjectResponse>>> GetProjectById(Guid id)
    {
        try
        {
            var project = await _projectService.GetProjectByIdAsync(id);
            if (project == null)
                return NotFound(ApiResponse<ProjectResponse>.Fail("Project not found"));

            var projectResponse = new ProjectResponse
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description,
                OwnerId = project.OwnerId,
                OwnerName = project.Owner?.Username ?? string.Empty,
                CreatedAt = project.CreatedAt,
                UpdatedAt = project.UpdatedAt
            };
            return Ok(ApiResponse<ProjectResponse>.Ok(projectResponse));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<ProjectResponse>.Fail(ex.Message));
        }
    }

    [HttpGet("owner/{ownerId}")]
    public async Task<ActionResult<ApiResponse<IEnumerable<ProjectResponse>>>> GetProjectsByOwnerId(Guid ownerId)
    {
        try
        {
            var projects = await _projectService.GetProjectsByOwnerIdAsync(ownerId);
            var projectResponses = projects.Select(p => new ProjectResponse
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                OwnerId = p.OwnerId,
                OwnerName = p.Owner?.Username ?? string.Empty,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt
            });
            return Ok(ApiResponse<IEnumerable<ProjectResponse>>.Ok(projectResponses));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<IEnumerable<ProjectResponse>>.Fail(ex.Message));
        }
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<ProjectResponse>>> CreateProject([FromBody] CreateProjectRequest request)
    {
        try
        {
            var project = new Project
            {
                Name = request.Name,
                Description = request.Description,
                OwnerId = request.OwnerId
            };
            var createdProject = await _projectService.CreateProjectAsync(project);
            
            var projectResponse = new ProjectResponse
            {
                Id = createdProject.Id,
                Name = createdProject.Name,
                Description = createdProject.Description,
                OwnerId = createdProject.OwnerId,
                OwnerName = createdProject.Owner?.Username ?? string.Empty,
                CreatedAt = createdProject.CreatedAt,
                UpdatedAt = createdProject.UpdatedAt
            };
            return CreatedAtAction(nameof(GetProjectById), new { id = createdProject.Id }, ApiResponse<ProjectResponse>.Ok(projectResponse, "Project created successfully"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<ProjectResponse>.Fail(ex.Message));
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<ProjectResponse>>> UpdateProject(Guid id, [FromBody] UpdateProjectRequest request)
    {
        try
        {
            var project = new Project
            {
                Name = request.Name,
                Description = request.Description
            };
            var updatedProject = await _projectService.UpdateProjectAsync(id, project);
            
            var projectResponse = new ProjectResponse
            {
                Id = updatedProject.Id,
                Name = updatedProject.Name,
                Description = updatedProject.Description,
                OwnerId = updatedProject.OwnerId,
                OwnerName = updatedProject.Owner?.Username ?? string.Empty,
                CreatedAt = updatedProject.CreatedAt,
                UpdatedAt = updatedProject.UpdatedAt
            };
            return Ok(ApiResponse<ProjectResponse>.Ok(projectResponse, "Project updated successfully"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<ProjectResponse>.Fail(ex.Message));
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteProject(Guid id)
    {
        try
        {
            await _projectService.DeleteProjectAsync(id);
            return Ok(ApiResponse<object>.Ok(null, "Project deleted successfully"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<object>.Fail(ex.Message));
        }
    }
}
