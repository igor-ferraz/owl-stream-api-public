using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using OwlStream.Domain.Configs;
using OwlStream.Domain.Exceptions.Services;
using OwlStream.Domain.Models.Security;
using OwlStream.Domain.Repositories;
using OwlStream.Domain.Services.Application;
using BCryptNet = BCrypt.Net.BCrypt;

namespace OwlStream.Application.Services;

public class SecurityService : ISecurityService
{
    private readonly IUsersRepository _usersRepository;
    private readonly JwtConfig _jwtConfig;

    public SecurityService(IUsersRepository usersRepository, IOptions<JwtConfig> jwtConfig)
    {
        _usersRepository = usersRepository;
        _jwtConfig = jwtConfig.Value;
    }

    public async Task<SecurityUser> ValidateCredentials(string email, string password)
    {
        var user = await _usersRepository.GetSecurityUser(email);

        if (user == null)
        {
            throw new UserNotFoundException();
        }

        if (BCryptNet.Verify(password, user.Password))
        {
            return user;
        }
        else
        {
            return null;
        }
    }

    public TokenInfo GenerateToken(string id, string role)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_jwtConfig.SecretKey);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                    new Claim(ClaimTypes.NameIdentifier, id),
                    new Claim(ClaimTypes.Role, role)
            }),
            Expires = DateTime.UtcNow.AddMinutes(_jwtConfig.ExpirationInMinutes),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature
            )
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);

        return new TokenInfo
        {
            Name = _jwtConfig.Name,
            ExpirationInMinutes = _jwtConfig.ExpirationInMinutes,
            Token = tokenHandler.WriteToken(token),
        };
    }
}