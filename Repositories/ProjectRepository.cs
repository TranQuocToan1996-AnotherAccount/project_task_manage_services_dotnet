using Microsoft.EntityFrameworkCore;
using TaskManagement.Common;
using TaskManagement.Data;
using TaskManagement.Models;
using TaskManagement.Repositories.Interfaces;

namespace TaskManagement.Repositories;

public class ProjectRepository : IProjectRepository
{
    private readonly AppDbContext _context;

    public ProjectRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Project?> GetByIdAsync(Guid id)
    {
        return await _context.Projects
            .Include(p => p.Owner)
            .Include(p => p.Tasks)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<PagedResult<Project>> GetAllAsync(int pageNumber, int pageSize)
    {
        var query = _context.Projects.Include(p => p.Owner);
        var totalCount = await query.CountAsync();
        
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<Project>
        {
            Items = items,
            Pagination = new Pagination
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            }
        };
    }

    public async Task<IEnumerable<Project>> GetByOwnerIdAsync(Guid ownerId)
    {
        return await _context.Projects
            .Include(p => p.Owner)
            .Where(p => p.OwnerId == ownerId)
            .ToListAsync();
    }

    public async Task<Project> CreateAsync(Project project)
    {
        _context.Projects.Add(project);
        await _context.SaveChangesAsync();
        return project;
    }

    public async Task<Project> UpdateAsync(Project project)
    {
        project.UpdatedAt = DateTime.UtcNow;
        _context.Projects.Update(project);
        await _context.SaveChangesAsync();
        return project;
    }

    public async Task DeleteAsync(Guid id)
    {
        var project = await _context.Projects.FindAsync(id);
        if (project != null)
        {
            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.Projects.AnyAsync(p => p.Id == id);
    }
}
