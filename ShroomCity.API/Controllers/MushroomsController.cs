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

    [Authorize(Policy = "read:mushrooms")]
    [HttpGet]
    [Route("")]
    public IActionResult GetAllMushrooms([FromQuery] string? name, [FromQuery] int? min_stem_size, [FromQuery] int? max_stem_size, [FromQuery] int? min_cap_size, [FromQuery] int? max_cap_size, [FromQuery] string? color, [FromQuery] int pageSize = 25, [FromQuery] int pageNumber = 1)
        => Ok(_mushroomService.GetMushrooms(name, min_stem_size, max_stem_size, min_cap_size, max_cap_size, color, pageSize, pageNumber));
    
    [Authorize(Policy = "read:mushrooms")]
    [HttpGet]
    [Route("{id}", Name = "ReadMushroom")]
    public async Task<IActionResult> GetMushroomById(int id)
    {
        return Ok(await _mushroomService.GetMushroomById(id));
    }

    [Authorize(Policy = "write:mushrooms")]
    [HttpPut]
    [Route("{id}", Name = "ReadMushroom")]
    public async Task<IActionResult> UpdateMushroomById(int id, [FromBody] MushroomUpdateInputModel inputModel, [FromQuery] bool lookup)
    {
        var result = await _mushroomService.UpdateMushroomById(id, inputModel, lookup);
        if (result)
        {
            return Ok();
        }
        else
        {
            return StatusCode(StatusCodes.Status404NotFound);
        }
    }

    [Authorize(Policy = "read:mushrooms")]
    [HttpGet]
    [Route("lookup")]
    public async Task<IActionResult> GetLookupMushroomsAsync([FromQuery] int pageSize = 25, [FromQuery] int pageNumber = 1)
    {
        return Ok(await _mushroomService.GetLookupMushrooms(pageSize, pageNumber));
    }

    [Authorize(Policy = "write:mushrooms")]
    [HttpPost]
    [Route("")]
    public async Task<IActionResult> CreateMushroom([FromBody] MushroomInputModel inputModel)
    {
        var researcherEmailAddress = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        var newMushroomId = await _mushroomService.CreateMushroom(researcherEmailAddress, inputModel);
        return CreatedAtRoute("ReadMushroom", new { id = newMushroomId }, null);
    }

    [Authorize(Policy = "write:mushrooms")]
    [HttpDelete]
    [Route("{id}")]
    public async Task<IActionResult> DeleteMushroom(int id)
    {
        var result = await _mushroomService.DeleteMushroomById(id);
        if (result)
        {
            return Ok();
        }
        else
        {
            return StatusCode(StatusCodes.Status404NotFound);
        }
    }

    [Authorize(Policy = "write:mushrooms")]
    [HttpPost]
    [Route("{id}/research-entries")]
    public async Task<IActionResult> CreateResearchEntry(int id, [FromBody] ResearchEntryInputModel inputModel)
    {
        var researcherEmailAddress = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        var result = await _mushroomService.CreateResearchEntry(id, researcherEmailAddress, inputModel);
        if (result)
        {
            return StatusCode(201);
        }
        return StatusCode(400);
    }
}