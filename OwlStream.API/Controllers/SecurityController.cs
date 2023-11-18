using Microsoft.AspNetCore.Mvc;
using OwlStream.API.DTOs;
using OwlStream.Domain.Exceptions.Services;
using OwlStream.Domain.Services.Application;

namespace OwlStream.API.Controllers;

[ApiController]
[Route("[controller]")]
public class SecurityController : ControllerBase
{
    private readonly ISecurityService _securityService;

    public SecurityController(ISecurityService securityService)
    {
        _securityService = securityService;
    }

    // POST login
    /// <summary>
    /// [Public] Authentication
    /// </summary>
    /// <response code="200">Returns JWT token.</response>
    /// <response code="400">Email not found.</response>
    /// <response code="401">Invalid credentials.</response>
    [HttpPost("login")]
    public async Task<ActionResult<string>> Login(LoginRequest login)
    {
        try
        {
            var user = await _securityService.ValidateCredentials(login.Email, login.Password);

            if (user == null)
            {
                return Unauthorized("E-mail e/ou senha inválidos.");
            }
            else
            {
                var tokenInfo = _securityService.GenerateToken(user.Id, user.Role);

                HttpContext.Response.Cookies.Append(
                    tokenInfo.Name,
                    tokenInfo.Token,
                    new CookieOptions
                    {
                        Expires = DateTime.Now.AddMinutes(tokenInfo.ExpirationInMinutes),
                        HttpOnly = true,
                        Secure = true,
                        IsEssential = true,
                        SameSite = SameSiteMode.None
                    });

                return Ok();
            }
        }
        catch (UserNotFoundException)
        {
            return BadRequest("O e-mail informado não está cadastrado em nosso sistema.");
        }
    }
}