namespace OwlStream.Domain.Repositories;

public interface ISecurityRepository
{
    Task<string> Login(string email, string password);
}