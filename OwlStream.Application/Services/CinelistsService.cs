using OwlStream.Domain.Models.Cinelists;
using OwlStream.Domain.Repositories;
using OwlStream.Domain.Services.Application;

namespace OwlStream.Application.Services;

public class CinelistsService : ICinelistsService
{
    private readonly ICinelistsRepository _cinelistsRepository;

    public CinelistsService(ICinelistsRepository cinelistsRepository)
    {
        _cinelistsRepository = cinelistsRepository;
    }

    public async Task<IEnumerable<CinelistResult>> Get(bool activeOnly)
    {
        return await _cinelistsRepository.Get(activeOnly);
    }

    public async Task<string> Add(Cinelist cinelist)
    {
        return await _cinelistsRepository.Add(cinelist);
    }

    public async Task<bool> Update(CinelistUpdate cinelist)
    {
        return await _cinelistsRepository.Update(cinelist);
    }

    public async Task<bool> ChangeStatus(string id, bool status)
    {
        return await _cinelistsRepository.ChangeStatus(id, status);
    }

    public async Task<bool> LinkMovie(string id, string movieId)
    {
        return await _cinelistsRepository.LinkMovie(id, movieId);
    }
}