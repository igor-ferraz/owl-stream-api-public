using OwlStream.Domain.Models.Genres;
using OwlStream.Domain.Repositories;
using OwlStream.Domain.Services.Application;

namespace OwlStream.Application.Services;

public class GenresService : IGenresService
{
    private readonly IGenresRepository _genresRepository;

    public GenresService(IGenresRepository genresRepository)
    {
        _genresRepository = genresRepository;
    }

    public async Task<IEnumerable<GenreResult>> Get(bool mainOnly, bool activeOnly)
    {
        return await _genresRepository.Get(mainOnly, activeOnly);
    }

    public async Task<string> Add(Genre genre)
    {
        return await _genresRepository.Add(genre);
    }

    public async Task<bool> Update(GenreUpdate genre)
    {
        return await _genresRepository.Update(genre);
    }

    public async Task<bool> ChangeStatus(string id, bool status)
    {
        return await _genresRepository.ChangeStatus(id, status);
    }
}