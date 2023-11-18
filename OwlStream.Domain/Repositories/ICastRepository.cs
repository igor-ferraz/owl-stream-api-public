using OwlStream.Domain.Models.Cast;

namespace OwlStream.Domain.Repositories;

public interface ICastRepository
{
    Task<CastResult> Get(string id);
    Task<string> Add(CastAdd person);
    Task<bool> Update(CastUpdate person);
    Task<bool> Delete(string id);
}