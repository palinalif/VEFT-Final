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

    [HttpGet]
    [Route("")]
    public async Task<IActionResult> GetAllResearchers()
        => Ok(await _researcherService.GetAllResearchers());
    
    [HttpGet]
    [Route("{id}", Name = "ReadResearcher")]
    public async Task<IActionResult> GetResearcherByIdAsync(int id)
    {
        return Ok(await _researcherService.GetResearcherById(id));
    }

    [HttpPost]
    [Route("")]
    public async Task<IActionResult> CreateResearcher([FromBody] ResearcherInputModel inputModel)
    {
        // TODO: get researcher... name?
        var newResearcherId = await _researcherService.CreateResearcher("", inputModel);
        return CreatedAtRoute("ReadResearcher", new { id = newResearcherId }, null);
    }
}