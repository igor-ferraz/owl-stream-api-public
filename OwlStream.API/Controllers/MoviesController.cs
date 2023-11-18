using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OwlStream.Domain.Exceptions.Services;
using OwlStream.Domain.Models.Movies;
using OwlStream.Domain.Services.Infra;
using OwlStream.Domain.Services.Application;

namespace OwlStream.API.Controllers;

[ApiController]
[Route("[controller]")]
public class MoviesController : ControllerBase
{
    private readonly IMoviesService _moviesService;

    public MoviesController(IMoviesService moviesService)
    {
        _moviesService = moviesService;
    }

    // GET movies
    /// <summary>
    /// [Public] Get a list of movies (it should contain at least one query parameter)
    /// </summary>
    /// <response code="200">Returns a list of movies (empty list if no one was found or do not receive filters).</response>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MovieResult>>> Search(
        [FromQuery] string text,
        [FromQuery] int? genreId,
        [FromQuery] string cinelistId,
        [FromQuery] int limit
    )
    {
        var movies = await _moviesService.Search(text, genreId, cinelistId, limit);
        return Ok(movies);
    }

    // GET movies/{id}
    /// <summary>
    /// [Public] Get a specific movie
    /// </summary>
    /// <response code="200">Returns specific movie.</response>
    [HttpGet("{id}")]
    public async Task<ActionResult<MovieResult>> Get([FromRoute] string id)
    {
        var movie = await _moviesService.Get(id);
        return Ok(movie);
    }


    // POST movies
    /// <summary>
    /// [Admin] Creates one new movie
    /// </summary>
    /// <response code="200">Returns id from new movie.</response>
    /// <response code="400">Some mandatory movie data is missing.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="403">Current user does not have access to this endpoint.</response>
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<string>> Add([FromBody] MovieAdd movie)
    {
        try
        {
            var newId = await _moviesService.Add(movie);
            return Ok(newId);
        }
        catch (IncompleteModelException)
        {
            return BadRequest("Algum atributo obrigatório foi enviado vazio ou incorreto.");
        }
    }

    // PUT movies
    /// <summary>
    /// [Admin] Updates one movie by id
    /// </summary>
    /// <response code="200">Returns boolean indicating success (true) or failure (false).</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="403">Current user does not have access to this endpoint.</response>
    [Authorize(Roles = "Admin")]
    [HttpPut]
    public async Task<ActionResult<bool>> Update([FromBody] MovieUpdate movie)
    {
        var result = await _moviesService.Update(movie);
        return Ok(result);
    }

    // PATCH movies/{id}/active/{active}
    /// <summary>
    /// [Admin] Changes a movie active attribute by id
    /// </summary>
    /// <response code="200">Returns boolean indicating success (true) or failure (false).</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="403">Current user does not have access to this endpoint.</response>
    [Authorize(Roles = "Admin")]
    [HttpPatch("{id}/active/{active}")]
    public async Task<ActionResult<bool>> ChangeStatus(string id, bool active)
    {
        var result = await _moviesService.ChangeStatus(id, active);
        return Ok(result);
    }

    // POST movies/{id}/picture
    /// <summary>
    /// [Admin] Upload movie's picture
    /// </summary>
    /// <response code="200">Returns boolean indicating success (true) or failure (false).</response>
    /// <response code="400">Invalid file format.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="403">Current user does not have access to this endpoint.</response>
    [Authorize(Roles = "Admin")]
    [HttpPost("{id}/picture")]
    [DisableRequestSizeLimit]
    public async Task<ActionResult<bool>> UploadPicture([FromRoute] string id, IFormFile file)
    {
        try
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest();
            }

            if (HttpContext.Request.Form.Files.Count > 0)
            {
                var result = await _moviesService.UploadPicture(id, file);

                if (result)
                {
                    return Ok();
                }
            }

            return StatusCode(500);
        }
        catch (InvalidFileMimeTypeException)
        {
            return BadRequest("Formato de arquivo inválido.");
        }
        catch (Exception)
        {
            return StatusCode(500);
        }
    }

    // POST movies/{id}/file
    /// <summary>
    /// [Admin] Upload movie's file
    /// </summary>
    /// <response code="200">Returns boolean indicating success (true) or failure (false).</response>
    /// <response code="400">Invalid file format.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="403">Current user does not have access to this endpoint.</response>
    [Authorize(Roles = "Admin")]
    [HttpPost("{id}/file")]
    [DisableRequestSizeLimit]
    public async Task<ActionResult<bool>> UploadFile([FromRoute] string id, IFormFile file)
    {
        try
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest();
            }

            if (HttpContext.Request.Form.Files.Count > 0)
            {
                var result = await _moviesService.Upload(id, file);

                if (result)
                {
                    return Ok();
                }
            }

            return StatusCode(500);
        }
        catch (InvalidFileMimeTypeException)
        {
            return BadRequest("Formato de arquivo inválido.");
        }
        catch (Exception)
        {
            return StatusCode(500);
        }
    }
}