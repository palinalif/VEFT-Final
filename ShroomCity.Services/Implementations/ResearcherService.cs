using ShroomCity.Models.Dtos;
using ShroomCity.Models.InputModels;
using ShroomCity.Repositories.Interfaces;
using ShroomCity.Services.Interfaces;

namespace ShroomCity.Services.Implementations;

public class ResearcherService : IResearcherService
{
    private readonly IResearcherRepository _researcherRepository;

    public ResearcherService(IResearcherRepository researcherRepository)
    {
        _researcherRepository = researcherRepository;
    }


    public async Task<int?> CreateResearcher(string createdBy, ResearcherInputModel inputModel)
    {
        return await _researcherRepository.CreateResearcher(createdBy, inputModel);
    }

    public async Task<IEnumerable<ResearcherDto>?> GetAllResearchers()
    {
        return await _researcherRepository.GetAllResearchers();
    }

    public async Task<ResearcherDto?> GetResearcherByEmailAddress(string emailAddress)
    {
        return await _researcherRepository.GetResearcherByEmailAddress(emailAddress);
    }

    public async Task<ResearcherDto?> GetResearcherById(int id)
    {
        return await _researcherRepository.GetResearcherById(id);
    }
}