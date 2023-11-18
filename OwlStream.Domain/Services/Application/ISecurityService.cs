using OwlStream.Domain.Models.Security;

namespace OwlStream.Domain.Services.Application;

public interface ISecurityService
{
    Task<SecurityUser> ValidateCredentials(string email, string password);
    TokenInfo GenerateToken(string id, string role);
}