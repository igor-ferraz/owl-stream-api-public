using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OwlStream.Domain.Models.MovieRoles;
using OwlStream.Domain.Services.Application;

namespace OwlStream.API.Controllers;

[ApiController]
[Route("movies-roles")]
public class MovieRolesController : ControllerBase
{
    private readonly IMovieRolesService _movieRolesService;

    public MovieRolesController(IMovieRolesService movieRolesService)
    {
        _movieRolesService = movieRolesService;
    }

    // GET movie-roles
    /// <summary>
    /// [Public] Get a list of movie roles. It can be filtered by query parameters
    /// </summary>
    /// <response code="200">Returns a list of movie roles (empty list if no one was found).</response>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MovieRoleResult>>> Get([FromQuery] bool activeOnly = true)
    {
        var movieRoles = await _movieRolesService.Get(activeOnly);
        return Ok(movieRoles);
    }

    // POST movie-roles
    /// <summary>
    /// [Admin] Creates one new movie role
    /// </summary>
    /// <response code="200">Returns id from new movie role.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="403">Current user does not have access to this endpoint.</response>
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<string>> Add([FromBody] MovieRole movieRole)
    {
        var newId = await _movieRolesService.Add(movieRole);
        return Ok(newId);
    }

    // PUT movie-roles
    /// <summary>
    /// [Admin] Updates a single movie role by id
    /// </summary>
    /// <response code="200">Returns boolean indicating success (true) or failure (false).</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="403">Current user does not have access to this endpoint.</response>
    [Authorize(Roles = "Admin")]
    [HttpPut]
    public async Task<ActionResult<bool>> Update([FromBody] MovieRoleUpdate movieRole)
    {
        var result = await _movieRolesService.Update(movieRole);
        return Ok(result);
    }


    // PATCH movie-roles/{id}/active/{active}
    /// <summary>
    /// [Admin] Changes the movie role active attribute by id
    /// </summary>
    /// <response code="200">Returns boolean indicating success (true) or failure (false).</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="403">Current user does not have access to this endpoint.</response>
    [Authorize(Roles = "Admin")]
    [HttpPatch("{id}/active/{active}")]
    public async Task<ActionResult<bool>> ChangeStatus(string id, bool active)
    {
        var result = await _movieRolesService.ChangeStatus(id, active);
        return Ok(result);
    }
}
