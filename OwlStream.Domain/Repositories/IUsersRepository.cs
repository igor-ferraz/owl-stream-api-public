using OwlStream.Domain.Models.Users;
using OwlStream.Domain.Models.Security;

namespace OwlStream.Domain.Repositories;

public interface IUsersRepository
{
    Task<UserResult> Get(Dictionary<string, object> filters);
    Task<string> Add(UserAdd user);
    Task<bool> Update(UserUpdateInternal user);
    Task<bool> Delete(string id);
    Task<SecurityUser> GetSecurityUser(string email);
}