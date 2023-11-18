using OwlStream.Domain.Models.MovieRoles;

namespace OwlStream.Domain.Services.Application;

public interface IMovieRolesService
{
    Task<IEnumerable<MovieRoleResult>> Get(bool activeOnly);
    Task<string> Add(MovieRole movieRole);
    Task<bool> Update(MovieRoleUpdate movieRole);
    Task<bool> ChangeStatus(string id, bool status);
}