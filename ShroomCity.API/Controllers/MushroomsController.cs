using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShroomCity.Models.InputModels;
using ShroomCity.Services.Interfaces;

namespace ShroomCity.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MushroomsController : ControllerBase
{
    private readonly IMushroomService _mushroomService;

    public MushroomsController(IMushroomService mushroomService)
    {
        _mushroomService = mushroomService;
    }
    // TODO: Make all of these be authorized routes
    [Authorize(Policy = "read:mushrooms")]
    [HttpGet]
    [Route("")]
    public IActionResult GetAllMushrooms([FromQuery] string? name, [FromQuery] int? min_stem_size, [FromQuery] int? max_stem_size, [FromQuery] int? min_cap_size, [FromQuery] int? max_cap_size, [FromQuery] string? color, [FromQuery] int pageSize = 25, [FromQuery] int pageNumber = 1)
        => Ok(_mushroomService.GetMushrooms(name, min_stem_size, max_stem_size, min_cap_size, max_cap_size, color, pageSize, pageNumber));
    
    [Authorize(Policy = "read:mushrooms")]
    [HttpGet]
    [Route("{id}", Name = "ReadMushroom")]
    public IActionResult GetMushroomById(int id)
    {
        return Ok(_mushroomService.GetMushroomById(id));
    }

    //[Authorize(Policy = "read:mushrooms")]
    [HttpGet]
    [Route("lookup")]
    public IActionResult GetLookupMushrooms([FromQuery] int pageSize = 25, [FromQuery] int pageNumber = 1)
    {
        return Ok(_mushroomService.GetLookupMushrooms(pageSize, pageNumber));
    }

    [HttpPost]
    [Route("")]
    public IActionResult CreateMushroom([FromBody] MushroomInputModel inputModel)
    {
        // TODO: Get researcher address
        var newMushroomId = _mushroomService.CreateMushroom("", inputModel);
        return CreatedAtRoute("ReadMushroom", new { id = newMushroomId }, null);
    }

    [HttpDelete]
    [Route("{id}")]
    public IActionResult DeleteMushroom(int id)
    {
        _mushroomService.DeleteMushroomById(id);
        return Ok();
    }

    [HttpPost]
    [Route("{id}/research-entries")]
    public IActionResult CreateResearchEntry(int id, [FromBody] ResearchEntryInputModel inputModel)
    {
        // TODO: Get researcher address
        // var researcherEmailAddress = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        _mushroomService.CreateResearchEntry(id, "", inputModel);
        return StatusCode(201);
    }
}