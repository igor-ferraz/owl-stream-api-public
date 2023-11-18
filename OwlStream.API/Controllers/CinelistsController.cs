using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OwlStream.Domain.Models.Cinelists;
using OwlStream.Domain.Services.Application;

namespace OwlStream.API.Controllers;

[ApiController]
[Route("[controller]")]
public class CinelistsController : ControllerBase
{
    private readonly ICinelistsService _cinelistsService;

    public CinelistsController(ICinelistsService cinelistsService)
    {
        _cinelistsService = cinelistsService;
    }

    // GET cinelists
    /// <summary>
    /// [Public] Get a list of cinelists. It can be filtered by query parameters
    /// </summary>
    /// <response code="200">Returns a list of cinelists (empty list if no one was found).</response>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CinelistResult>>> Get([FromQuery] bool activeOnly = true)
    {
        var cinelists = await _cinelistsService.Get(activeOnly);
        return Ok(cinelists);
    }

    // POST cinelists
    /// <summary>
    /// [Admin] Creates one new cinelist
    /// </summary>
    /// <response code="200">Returns id from new cinelist.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="403">Current user does not have access to this endpoint.</response>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<string>> Add([FromBody] Cinelist cinelist)
    {
        var newId = await _cinelistsService.Add(cinelist);
        return Ok(newId);
    }

    // PUT cinelists
    /// <summary>
    /// [Admin] Updates a single cinelist by id
    /// </summary>
    /// <response code="200">Returns boolean indicating success (true) or failure (false).</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="403">Current user does not have access to this endpoint.</response>
    [HttpPut]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<bool>> Update([FromBody] CinelistUpdate cinelist)
    {
        var result = await _cinelistsService.Update(cinelist);
        return Ok(result);
    }


    // PATCH cinelists/{id}/active/{active}
    /// <summary>
    /// [Admin] Changes the cinelist active attribute by id
    /// </summary>
    /// <response code="200">Returns boolean indicating success (true) or failure (false).</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="403">Current user does not have access to this endpoint.</response>
    [HttpPatch("{id}/active/{active}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<bool>> ChangeStatus(string id, bool active)
    {
        var result = await _cinelistsService.ChangeStatus(id, active);
        return Ok(result);
    }

    // POST cinelists/{id}/movies/{id}
    /// <summary>
    /// [Admin] Link cinelist to movie by ids
    /// </summary>
    /// <response code="200">Returns boolean indicating success (true) or failure (false).</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="403">Current user does not have access to this endpoint.</response>
    [HttpPost("{id}/movies/{movieId}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<bool>> LinkMovie(string id, string movieId)
    {
        var result = await _cinelistsService.LinkMovie(id, movieId);
        return Ok(result);
    }
}