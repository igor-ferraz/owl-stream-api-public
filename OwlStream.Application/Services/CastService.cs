using OwlStream.Domain.Models.Cast;
using OwlStream.Domain.Repositories;
using OwlStream.Domain.Services.Application;

namespace OwlStream.Application.Services;

public class CastService : ICastService
{
    private readonly ICastRepository _castRepository;

    public CastService(ICastRepository castRepository)
    {
        _castRepository = castRepository;
    }

    public async Task<CastResult> Get(string id)
    {
        return await _castRepository.Get(id);
    }

    public async Task<string> Add(CastAdd person)
    {
        var id = await _castRepository.Add(person);
        // var result = await _azureStorageService.Upload(person.Picture, id);

        // if (!result)
        // {
        //     await _castRepository.Delete(id);
        //     return null;
        // }

        return id;
    }

    public async Task<bool> Update(CastUpdate person)
    {
        return await _castRepository.Update(person);
    }

    public async Task<bool> Delete(string id)
    {
        return await _castRepository.Delete(id);
    }
}
