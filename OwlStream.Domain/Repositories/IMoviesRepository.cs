using OwlStream.Domain.Models.Movies;

namespace OwlStream.Domain.Repositories;

public interface IMoviesRepository
{
    Task<IEnumerable<MovieResult>> Search(string text, int? genreId, string cinelistId, int limit);
    Task<MovieResult> Get(string id);
    Task<string> Add(MovieAdd movie);
    Task<bool> Update(MovieUpdate movie);
    Task<bool> ChangeStatus(string id, bool status);
    Task<bool> AddUrl(string id, string column, string url);
}
