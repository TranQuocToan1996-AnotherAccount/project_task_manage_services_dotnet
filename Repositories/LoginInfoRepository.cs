using Microsoft.EntityFrameworkCore;
using TaskManagement.Data;
using TaskManagement.Models;
using TaskManagement.Repositories.Interfaces;

namespace TaskManagement.Repositories;

public class LoginInfoRepository : ILoginInfoRepository
{
    private readonly AppDbContext _context;

    public LoginInfoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<LoginInfo> CreateAsync(LoginInfo loginInfo)
    {
        _context.LoginInfos.Add(loginInfo);
        await _context.SaveChangesAsync();
        return loginInfo;
    }
}
