using OwlStream.Domain.Models.Genres;

namespace OwlStream.Domain.Services.Application;

public interface IGenresService
{
    Task<IEnumerable<GenreResult>> Get(bool mainOnly, bool activeOnly);
    Task<string> Add(Genre genre);
    Task<bool> Update(GenreUpdate genre);
    Task<bool> ChangeStatus(string id, bool status);
}