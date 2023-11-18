using OwlStream.Domain.Models.Cinelists;

namespace OwlStream.Domain.Repositories;

public interface ICinelistsRepository
{
    Task<IEnumerable<CinelistResult>> Get(bool activeOnly);
    Task<string> Add(Cinelist cinelist);
    Task<bool> Update(CinelistUpdate cinelist);
    Task<bool> ChangeStatus(string id, bool status);
    Task<bool> LinkMovie(string id, string movieId);
}