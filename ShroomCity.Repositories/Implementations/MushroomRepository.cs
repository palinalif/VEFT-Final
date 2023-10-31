using Microsoft.EntityFrameworkCore;
using ShroomCity.Models.Dtos;
using ShroomCity.Models.Entities;
using ShroomCity.Models.InputModels;
using ShroomCity.Repositories.Interfaces;
using Attribute = ShroomCity.Models.Entities.Attribute;

namespace ShroomCity.Repositories.Implementations;

public class MushroomRepository : IMushroomRepository
{
    ShroomCityDbContext _dbContext;

    public MushroomRepository(ShroomCityDbContext dbContext)
    {
        _dbContext = dbContext;
    }


    public async Task<int> CreateMushroom(MushroomInputModel mushroom, string researcherEmailAddress, List<AttributeDto> attributes)
    {
        var attributeEntities = new List<Attribute>();
        foreach (var attribute in attributes)
        {
            var user = _dbContext.Users.FirstOrDefault(u => u.EmailAddress == researcherEmailAddress);
            if (user == null)
            {
                throw new Exception("Researcher does not exist");
            }
            var attributeType = _dbContext.Attributes.FirstOrDefault(a => a.Id == attribute.Id).AttributeType; // does this work??
            attributeEntities.Add(new Attribute
            {
                Id = attribute.Id,
                Value = attribute.Value,
                AttributeType = attributeType, // find attribute with that name
                RegisteredBy = user // find user with researcherEmailAddress
            });
        }

        var entity = new Mushroom
        {
            Name = mushroom.Name,
            Description = mushroom.Description,
            Attributes = attributeEntities
        };

        _dbContext.Mushrooms.Add(entity);
        _dbContext.SaveChanges();
        return entity.Id;
    }

    public async Task<bool> CreateResearchEntry(int mushroomId, string researcherEmailAddress, ResearchEntryInputModel inputModel)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> DeleteMushroomById(int mushroomId)
    {
        var mushroom = _dbContext.Mushrooms.FirstOrDefault(m => m.Id == mushroomId);
        if (mushroom == null)
        {
            return false;
        }
        _dbContext.Mushrooms.Remove(mushroom);
        _dbContext.SaveChanges();
        return true;
    }

    public async Task<MushroomDetailsDto?> GetMushroomById(int id)
    {
        var mushroom = _dbContext.Mushrooms
        .Include(m => m.Attributes)
            .ThenInclude(a => a.AttributeType)
        .Include(m => m.Attributes)
            .ThenInclude(a => a.RegisteredBy)
        .FirstOrDefault(m => m.Id == id);

        if (mushroom == null)
        {
            return null;
        }
        var mushroomDto = new MushroomDetailsDto
        {
            Id = mushroom.Id,
            Name = mushroom.Name,
            Description = mushroom.Description,
            Attributes = mushroom.Attributes.Select(a => new AttributeDto
            {
                Id = a.Id,
                Value = a.Value,
                Type = a.AttributeType.Type,
                RegisteredBy = a.RegisteredBy.EmailAddress,
                RegistrationDate = a.RegisteredBy.RegistrationDate
            })
        };
        _dbContext.Mushrooms.Add(mushroom);
        _dbContext.SaveChanges();
        return mushroomDto;
    }

    public (int totalPages, IEnumerable<MushroomDto> mushrooms) GetMushroomsByCriteria(string? name, int? stemSizeMinimum, int? stemSizeMaximum, int? capSizeMinimum, int? capSizeMaximum, string? color, int pageSize, int pageNumber)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> UpdateMushroomById(int mushroomId, MushroomUpdateInputModel inputModel)
    {
        var mushroom = _dbContext.Mushrooms.FirstOrDefault(m => m.Id == mushroomId);
        if (mushroom == null)
        {
            return false;
        }
        if (inputModel.Name != null)
        {
            mushroom.Name = inputModel.Name;
        }
        if (inputModel.Description != null)
        {
            mushroom.Description = inputModel.Description;
        }
        _dbContext.SaveChanges();
        return true;
    }
}