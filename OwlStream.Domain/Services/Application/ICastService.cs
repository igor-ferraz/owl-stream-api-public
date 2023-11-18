using OwlStream.Domain.Models.Cast;

namespace OwlStream.Domain.Services.Application;

public interface ICastService
{
    Task<CastResult> Get(string id);
    Task<string> Add(CastAdd person);
    Task<bool> Update(CastUpdate person);
    Task<bool> Delete(string id);
}