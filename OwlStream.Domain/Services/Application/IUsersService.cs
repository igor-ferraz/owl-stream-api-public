using OwlStream.Domain.Models.Users;

namespace OwlStream.Domain.Services.Application;

public interface IUsersService
{
    Task<UserResult> Get(string id);
    Task<string> Add(UserAdd user);
    Task<bool> Update(UserUpdateInternal user);
    Task<bool> Delete(string id);
}