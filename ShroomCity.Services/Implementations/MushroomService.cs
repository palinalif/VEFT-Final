using Microsoft.AspNetCore.Http.Features;
using ShroomCity.Models;
using ShroomCity.Models.Dtos;
using ShroomCity.Models.InputModels;
using ShroomCity.Repositories.Interfaces;
using ShroomCity.Services.Interfaces;

namespace ShroomCity.Services.Implementations;

public class MushroomService : IMushroomService
{
    private readonly IExternalMushroomService _externalMushroomService;
    private readonly IMushroomRepository _mushroomRepository;

    public MushroomService(IExternalMushroomService externalMushroomService, IMushroomRepository mushroomRepository)
    {
        _externalMushroomService = externalMushroomService;
        _mushroomRepository = mushroomRepository;
    }


    public async Task<int> CreateMushroom(string researcherEmailAddress, MushroomInputModel inputModel)
    {
        // fetches the mushroom from the external service and parses its attributes to AttributeDtos
        var mushroom = await _externalMushroomService.GetMushroomByName(inputModel.Name);
        if (mushroom == null)
        {
            // Create mushroom only with given values
            throw new NotImplementedException();
        }
        Console.WriteLine("External mushroom found!");
        Console.WriteLine(mushroom.Name);
        Console.WriteLine(mushroom.Description);

        var attributes = new List<AttributeDto>();
        foreach (var color in mushroom.Colors)
        {
            attributes.Add(new AttributeDto
            {
                Value = color,
                Type = "Color",
                RegistrationDate = DateTime.Now
            });
        }

        foreach (var surface in mushroom.Surfaces)
        {
            attributes.Add(new AttributeDto
            {
                Value = surface,
                Type = "Surface",
                RegistrationDate = DateTime.Now
            });
        }

        foreach (var shape in mushroom.Shapes)
        {
            attributes.Add(new AttributeDto
            {
                Value = shape,
                Type = "Shape",
                RegistrationDate = DateTime.Now
            });
        }

        return await _mushroomRepository.CreateMushroom(inputModel, researcherEmailAddress, attributes);
    }

    public async Task<bool> CreateResearchEntry(int mushroomId, string researcherEmailAddress, ResearchEntryInputModel inputModel)
    {
        return await _mushroomRepository.CreateResearchEntry(mushroomId, researcherEmailAddress, inputModel);
    }

    public async Task<bool> DeleteMushroomById(int mushroomId)
    {
        return await _mushroomRepository.DeleteMushroomById(mushroomId);
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

    public async Task<MushroomDetailsDto?> GetMushroomById(int id)
    {
        return await _mushroomRepository.GetMushroomById(id);
    }

    public Envelope<MushroomDto>? GetMushrooms(string? name, int? stemSizeMinimum, int? stemSizeMaximum, int? capSizeMinimum, int? capSizeMaximum, string? color, int pageSize, int pageNumber)
    {
        (int totalPages, IEnumerable<MushroomDto> mushrooms) result = _mushroomRepository.GetMushroomsByCriteria(
            name,
            stemSizeMinimum,
            stemSizeMaximum,
            capSizeMinimum,
            capSizeMaximum,
            color,
            pageSize,
            pageNumber
        );
    
        int totalPagesValue = result.totalPages;
        IEnumerable<MushroomDto> mushroomsValue = result.mushrooms;

        var envelope = new Envelope<MushroomDto>
        {
            PageSize = pageSize,
            PageNumber = pageNumber,
            TotalPages = totalPagesValue,
            Items = mushroomsValue
        };
        return envelope;
    }

    public async Task<bool> UpdateMushroomById(int mushroomId, MushroomUpdateInputModel inputModel, bool performLookup)
    {
        if (performLookup)
        {
            // lookup should be performed to retrieve values from the external API to fill in the name, description and other attributes
            //(make new mushroom input model and discard the old one aside from the name?)
            if (inputModel.Name != null)
            {
                var lookupMushroom = await _externalMushroomService.GetMushroomByName(inputModel.Name);
                if (lookupMushroom != null)
                {
                    inputModel.Description = lookupMushroom.Description;
                }
            }
        }
        return await _mushroomRepository.UpdateMushroomById(mushroomId, inputModel);
    }
}