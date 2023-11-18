using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OwlStream.Domain.Models.Cast;
using OwlStream.Domain.Services.Application;

namespace OwlStream.API.Controllers;

[ApiController]
[Route("[controller]")]
public class CastController : ControllerBase
{
    private readonly ICastService _castService;

    public CastController(ICastService castService)
    {
        _castService = castService;
    }

    // GET cast
    /// <summary>
    /// [Public] Get a specific cast by id
    /// </summary>
    /// <response code="200">Returns specific cast.</response>
    [HttpGet("{id}")]
    public async Task<ActionResult<CastResult>> Get([FromRoute] string id)
    {
        var person = await _castService.Get(id);
        return Ok(person);
    }

    // POST cast
    /// <summary>
    /// [Admin] Creates one new person
    /// </summary>
    /// <response code="200">Returns id from new person.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="403">Current user does not have access to this endpoint.</response>
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<string>> Add([FromBody] CastAdd person)
    {
        var newId = await _castService.Add(person);
        return Ok(newId);
    }

    // PUT cast
    /// <summary>
    /// [Admin] Updates a single person by id
    /// </summary>
    /// <response code="200">Returns boolean indicating success (true) or failure (false).</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="403">Current user does not have access to this endpoint.</response>
    [Authorize(Roles = "Admin")]
    [HttpPut]
    public async Task<ActionResult<bool>> Update([FromBody] CastUpdate person)
    {
        var result = await _castService.Update(person);
        return Ok(result);
    }


    // DELETE cast
    /// <summary>
    /// [Admin] Deletes a single person by id
    /// </summary>
    /// <response code="200">Returns boolean indicating success (true) or failure (false).</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="403">Current user does not have access to this endpoint.</response>
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<ActionResult<bool>> Delete(string id)
    {
        var result = await _castService.Delete(id);
        return Ok(result);
    }
}