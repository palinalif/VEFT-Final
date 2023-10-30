using Microsoft.AspNetCore.Http.Features;
using ShroomCity.Models;
using ShroomCity.Models.Dtos;
using ShroomCity.Models.InputModels;
using ShroomCity.Services.Interfaces;

namespace ShroomCity.Services.Implementations;

public class MushroomService : IMushroomService
{
    private readonly IExternalMushroomService _externalMushroomService;

    public MushroomService(IExternalMushroomService externalMushroomService)
    {
        _externalMushroomService = externalMushroomService;
    }


    public Task<int> CreateMushroom(string researcherEmailAddress, MushroomInputModel inputModel)
    {
        throw new NotImplementedException();
    }

    public Task<bool> CreateResearchEntry(int mushroomId, string researcherEmailAddress, ResearchEntryInputModel inputModel)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteMushroomById(int mushroomId)
    {
        throw new NotImplementedException();
    }

    public async Task<Envelope<MushroomDto>?> GetLookupMushrooms(int pageSize, int pageNumber)
    {
        var externalMushroomEnvelope = await _externalMushroomService.GetMushrooms(pageSize, pageNumber);
        // turn all mushrooms into normal mushroom dtos
        var new_items = externalMushroomEnvelope.Items.Select(m => new MushroomDto
        {
            Name = m.Name,
            Description = m.Description
        });
        var newEnvelope = new Envelope<MushroomDto>
        {
            PageNumber = externalMushroomEnvelope.PageNumber,
            PageSize = externalMushroomEnvelope.PageSize,
            TotalPages = externalMushroomEnvelope.TotalPages,
            Items = new_items
        };
        return newEnvelope;
    }

    public Task<MushroomDetailsDto?> GetMushroomById(int id)
    {
        throw new NotImplementedException();
    }

    public Envelope<MushroomDto>? GetMushrooms(string? name, int? stemSizeMinimum, int? stemSizeMaximum, int? capSizeMinimum, int? capSizeMaximum, string? color, int pageSize, int pageNumber)
    {
        throw new NotImplementedException();
    }

    public Task<bool> UpdateMushroomById(int mushroomId, MushroomUpdateInputModel inputModel, bool performLookup)
    {
        throw new NotImplementedException();
    }
}