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
    public IActionResult GetAllResearchers()
        => Ok(_researcherService.GetAllResearchers());
    
    [HttpGet]
    [Route("{id}", Name = "ReadResearcher")]
    public IActionResult GetResearcherById(int id)
    {
        return Ok(_researcherService.GetResearcherById(id));
    }

    [HttpPost]
    [Route("")]
    public IActionResult CreateResearcher([FromBody] ResearcherInputModel inputModel)
    {
        // TODO: get researcher... name?
        var newResearcherId = _researcherService.CreateResearcher("", inputModel);
        return CreatedAtRoute("ReadResearcher", new { id = newResearcherId }, null);
    }
}