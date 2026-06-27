using TaskManagement.Models;

namespace TaskManagement.Repositories.Interfaces;

public interface ILoginInfoRepository
{
    Task<LoginInfo> CreateAsync(LoginInfo loginInfo);
}
