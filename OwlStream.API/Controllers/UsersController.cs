using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OwlStream.Domain.Exceptions.Infra;
using OwlStream.Domain.Models.Users;
using OwlStream.Domain.Services.Application;

namespace OwlStream.API.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUsersService _usersService;

    public UsersController(IUsersService usersService)
    {
        _usersService = usersService;
    }

    // GET Users
    /// <summary>
    /// [Private] Get data from current user
    /// </summary>
    /// <response code="200">Returns user data.</response>
    /// <response code="204">User not found.</response>
    /// <response code="401">Unauthorized.</response>
    [Authorize]
    [HttpGet]
    public async Task<ActionResult<UserResult>> Get()
    {
        var id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(id))
        {
            return BadRequest("Não foi possível validar sua identidade. Tente realizar logoff e tentar novamente.");
        }

        var user = await _usersService.Get(id);
        return Ok(user);
    }

    // POST Users
    /// <summary>
    /// [Public] Creates a new user
    /// </summary>
    /// <response code="200">Returns id from new user.</response>
    /// <response code="400">An error ocurred. User was not created.</response>
    [HttpPost]
    public async Task<ActionResult<string>> Add([FromBody] UserAdd user)
    {
        try
        {
            var newId = await _usersService.Add(user);

            return Ok(newId);
        }
        catch (DuplicatedEmailException)
        {
            return BadRequest("O e-mail informado já está cadastrado em nossa base.");
        }
    }

    // PUT Users
    /// <summary>
    /// [Private] Updates current user data
    /// </summary>
    /// <response code="200">Returns boolean indicating success (true) or failure (false).</response>
    /// <response code="400">It was not possible to validate your JWT token.</response>
    /// <response code="401">Unauthorized.</response>
    [Authorize]
    [HttpPut]
    public async Task<ActionResult<bool>> Update([FromBody] UserUpdate user)
    {
        var id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(id))
        {
            return BadRequest("Não foi possível validar sua identidade. Tente realizar logoff e tentar novamente.");
        }

        var userInternal = new UserUpdateInternal(user, id);

        try
        {
            var result = await _usersService.Update(userInternal);
            return Ok(result);
        }
        catch (DuplicatedEmailException)
        {
            return BadRequest("O e-mail informado já está cadastrado em nossa base.");
        }
    }

    // DELETE Users
    /// <summary>
    /// [Private] Deletes current user
    /// </summary>
    /// <response code="200">Returns boolean indicating success (true) or failure (false).</response>
    /// <response code="401">Unauthorized.</response>
    [Authorize]
    [HttpDelete]
    public async Task<ActionResult<bool>> Delete()
    {
        var id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(id))
        {
            return BadRequest("Não foi possível validar sua identidade. Tente realizar logoff e tentar novamente.");
        }

        var result = await _usersService.Delete(id);
        return Ok(result);
    }
}
