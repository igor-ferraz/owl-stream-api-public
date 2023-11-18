using OwlStream.Domain.Models.People;

namespace OwlStream.Domain.Repositories;

public interface IPeopleRepository
{
    Task<string> Add(Person person);
    Task<bool> Delete(string id);
}