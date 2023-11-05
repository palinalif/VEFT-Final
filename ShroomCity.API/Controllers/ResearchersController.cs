using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShroomCity.Models.InputModels;
using ShroomCity.Services.Interfaces;

namespace ShroomCity.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ResearchersController : ControllerBase
{
    private readonly IResearcherService _researcherService;

    public ResearchersController(IResearcherService researcherService)
    {
        _researcherService = researcherService;
    }

    [Authorize(Policy = "read:researchers")]
    [HttpGet]
    [Route("")]
    public async Task<IActionResult> GetAllResearchers()
        => Ok(await _researcherService.GetAllResearchers());
    
    [Authorize(Policy = "read:researchers")]
    [HttpGet]
    [Route("{id}", Name = "ReadResearcher")]
    public async Task<IActionResult> GetResearcherByIdAsync(int id)
    {
        return Ok(await _researcherService.GetResearcherById(id));
    }

    [Authorize(Policy = "write:researchers")]
    [HttpPost]
    [Route("")]
    public async Task<IActionResult> CreateResearcher([FromBody] ResearcherInputModel inputModel)
    {
        // TODO: get researcher... name?
        var userName = User.Identity.Name;
        try
        {
            var newResearcherId = await _researcherService.CreateResearcher(userName, inputModel);
            return CreatedAtRoute("ReadResearcher", new { id = newResearcherId }, null);
        }
        catch (System.Exception)
        {
            return StatusCode(StatusCodes.Status400BadRequest);
        }
    }
}