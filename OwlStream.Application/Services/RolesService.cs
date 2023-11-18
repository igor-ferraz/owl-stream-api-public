using OwlStream.Domain.Models.MovieRoles;
using OwlStream.Domain.Repositories;
using OwlStream.Domain.Services.Application;

namespace OwlStream.Application.Services;
public class MovieRolesService : IMovieRolesService
{
    private readonly IMovieRolesRepository _movieRolesRepository;

    public MovieRolesService(IMovieRolesRepository movieRolesRepository)
    {
        _movieRolesRepository = movieRolesRepository;
    }

    public async Task<IEnumerable<MovieRoleResult>> Get(bool activeOnly)
    {
        return await _movieRolesRepository.Get(activeOnly);
    }

    public async Task<string> Add(MovieRole movieRole)
    {
        return await _movieRolesRepository.Add(movieRole);
    }

    public async Task<bool> Update(MovieRoleUpdate movieRole)
    {
        return await _movieRolesRepository.Update(movieRole);
    }

    public async Task<bool> ChangeStatus(string id, bool status)
    {
        return await _movieRolesRepository.ChangeStatus(id, status);
    }
}