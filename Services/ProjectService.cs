using TaskManagement.Common;
using TaskManagement.Constants;
using TaskManagement.Models;
using TaskManagement.Repositories.Interfaces;
using TaskManagement.Services.Interfaces;
using TaskManagement.Utility;

namespace TaskManagement.Services;

public class ProjectService : IProjectService
{
    private readonly IProjectRepository _projectRepository;
    private readonly IUserRepository _userRepository;
    private readonly IRedisHelper _redisHelper;

    public ProjectService(IProjectRepository projectRepository, IUserRepository userRepository, IRedisHelper redisHelper)
    {
        _projectRepository = projectRepository;
        _userRepository = userRepository;
        _redisHelper = redisHelper;
    }

    public async Task<Project?> GetProjectByIdAsync(Guid id)
    {
        var project = await _projectRepository.GetByIdAsync(id) ??
            throw new KeyNotFoundException($"Project with id {id} not found");
        return project;
    }

    public async Task<PagedResult<Project>> GetAllProjectsAsync(int pageNumber, int pageSize)
    {
        return await _projectRepository.GetAllAsync(pageNumber, pageSize);
    }

    public async Task<IEnumerable<Project>> GetProjectsByOwnerIdAsync(Guid ownerId)
    {
        var ownerExists = await _userRepository.ExistsAsync(ownerId);
        if (!ownerExists)
            throw new KeyNotFoundException($"User with id {ownerId} not found");

        return await _projectRepository.GetByOwnerIdAsync(ownerId);
    }

    public async Task<Project> CreateProjectAsync(Project project)
    {
        var owner = await _userRepository.GetByIdAsync(project.OwnerId) ?? throw new KeyNotFoundException($"Owner with id {project.OwnerId} not found");
        project.Owner = owner;

        return await _projectRepository.CreateAsync(project);
    }

    public async Task<Project> UpdateProjectAsync(Guid id, Project project)
    {
        var existingProject = await _projectRepository.GetByIdAsync(id);
        if (existingProject == null)
            throw new KeyNotFoundException($"Project with id {id} not found");

        existingProject.Name = project.Name;
        existingProject.Description = project.Description;

        var updatedProject = await _projectRepository.UpdateAsync(existingProject);
        
        // Invalidate cache
        var cacheKey = CacheKeys.Project(id);
        await _redisHelper.RemoveAsync(cacheKey);
        
        return updatedProject;
    }

    public async Task DeleteProjectAsync(Guid id)
    {
        var exists = await _projectRepository.ExistsAsync(id);
        if (!exists)
            throw new KeyNotFoundException($"Project with id {id} not found");

        await _projectRepository.DeleteAsync(id);
        
        // Invalidate cache
        var cacheKey = CacheKeys.Project(id);
        await _redisHelper.RemoveAsync(cacheKey);
    }
}
