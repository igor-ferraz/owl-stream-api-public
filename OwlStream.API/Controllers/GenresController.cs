using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OwlStream.Domain.Models.Genres;
using OwlStream.Domain.Services.Application;

namespace OwlStream.API.Controllers;

[ApiController]
[Route("[controller]")]
public class GenresController : ControllerBase
{
    private readonly IGenresService _genresService;

    public GenresController(IGenresService genresService)
    {
        _genresService = genresService;
    }

    // GET genres
    /// <summary>
    /// [Public] Get a list of genres. It can be filtered by query parameters
    /// </summary>
    /// <response code="200">Returns a list of genres (empty list if no one was found).</response>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<GenreResult>>> Get(
        [FromQuery] bool mainOnly = true,
        [FromQuery] bool activeOnly = true
    )
    {
        var genres = await _genresService.Get(mainOnly, activeOnly);
        return Ok(genres);
    }

    // POST genres
    /// <summary>
    /// [Admin] Creates one new genre
    /// </summary>
    /// <response code="200">Returns id from new genre.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="403">Current user does not have access to this endpoint.</response>
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<string>> Add([FromBody] Genre genre)
    {
        var newId = await _genresService.Add(genre);
        return Ok(newId);
    }

    // PUT genres
    /// <summary>
    /// [Admin] Updates a single genre by id
    /// </summary>
    /// <response code="200">Returns boolean indicating success (true) or failure (false).</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="403">Current user does not have access to this endpoint.</response>
    [Authorize(Roles = "Admin")]
    [HttpPut]
    public async Task<ActionResult<bool>> Update([FromBody] GenreUpdate genre)
    {
        var result = await _genresService.Update(genre);
        return Ok(result);
    }


    // PATCH genres/{id}/status/{status}
    /// <summary>
    /// [Admin] Changes the genre active attribute by id
    /// </summary>
    /// <response code="200">Returns boolean indicating success (true) or failure (false).</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="403">Current user does not have access to this endpoint.</response>
    [Authorize(Roles = "Admin")]
    [HttpPatch("{id}/active/{active}")]
    public async Task<ActionResult<bool>> ChangeStatus(string id, bool active)
    {
        var result = await _genresService.ChangeStatus(id, active);
        return Ok(result);
    }
}
