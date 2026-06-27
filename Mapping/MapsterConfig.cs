using Mapster;
using TaskManagement.DTO.Project;
using TaskManagement.DTO.Task;
using TaskManagement.DTO.User;
using TaskManagement.Models;

namespace TaskManagement.Mapping;

public static class MapsterConfig
{
    public static void ConfigureMapster()
    {
        // User mappings
        TypeAdapterConfig<User, UserResponse>
            .NewConfig()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.Username, src => src.Username)
            .Map(dest => dest.Email, src => src.Email)
            .Map(dest => dest.CreatedAt, src => src.CreatedAt)
            .Map(dest => dest.UpdatedAt, src => src.UpdatedAt);

        // Project mappings
        TypeAdapterConfig<Project, ProjectResponse>
            .NewConfig()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.Name, src => src.Name)
            .Map(dest => dest.Description, src => src.Description)
            .Map(dest => dest.OwnerId, src => src.OwnerId)
            .Map(dest => dest.OwnerName, src => src.Owner != null ? src.Owner.Username : string.Empty)
            .Map(dest => dest.CreatedAt, src => src.CreatedAt)
            .Map(dest => dest.UpdatedAt, src => src.UpdatedAt);

        // Task mappings
        TypeAdapterConfig<TaskItem, TaskResponse>
            .NewConfig()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.Title, src => src.Title)
            .Map(dest => dest.Description, src => src.Description)
            .Map(dest => dest.Status, src => src.Status)
            .Map(dest => dest.Priority, src => src.Priority)
            .Map(dest => dest.ProjectId, src => src.ProjectId)
            .Map(dest => dest.ProjectName, src => src.Project != null ? src.Project.Name : string.Empty)
            .Map(dest => dest.AssigneeId, src => src.AssigneeId)
            .Map(dest => dest.AssigneeName, src => src.Assignee != null ? src.Assignee.Username : null)
            .Map(dest => dest.CreatedAt, src => src.CreatedAt)
            .Map(dest => dest.UpdatedAt, src => src.UpdatedAt);
    }
}
