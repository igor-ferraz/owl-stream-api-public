using BCryptNet = BCrypt.Net.BCrypt;
using OwlStream.Domain.Models.Users;
using OwlStream.Domain.Repositories;
using OwlStream.Domain.Services.Application;

namespace OwlStream.Application.Services;

public class UsersService : IUsersService
{
    private readonly IUsersRepository _usersRepository;

    public UsersService(IUsersRepository usersRepository)
    {
        _usersRepository = usersRepository;
    }

    public async Task<UserResult> Get(string id)
    {
        var filters = new Dictionary<string, object>
            {
                { "Id", id }
            };

        return await _usersRepository.Get(filters);
    }

    public async Task<string> Add(UserAdd user)
    {
        user.Password = BCryptNet.HashPassword(user.Password);
        return await _usersRepository.Add(user);
    }

    public async Task<bool> Update(UserUpdateInternal user)
    {
        return await _usersRepository.Update(user);
    }

    public async Task<bool> Delete(string id)
    {
        return await _usersRepository.Delete(id);
    }
}