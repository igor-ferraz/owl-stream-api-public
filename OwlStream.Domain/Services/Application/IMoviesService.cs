using Microsoft.AspNetCore.Http;
using OwlStream.Domain.Models.Movies;

namespace OwlStream.Domain.Services.Application;

public interface IMoviesService
{
    Task<IEnumerable<MovieResult>> Search(string text, int? genreId, string cinelistId, int limit);
    Task<Movie> Get(string id);
    Task<string> Add(MovieAdd movie);
    Task<bool> Update(MovieUpdate movie);
    Task<bool> ChangeStatus(string id, bool status);
    Task<bool> Upload(string id, IFormFile file);
    Task<bool> UploadPicture(string id, IFormFile file);
}