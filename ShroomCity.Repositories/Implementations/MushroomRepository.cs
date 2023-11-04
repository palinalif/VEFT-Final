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
            // find attributeType by name based on attributeDto's Type
            var attributeType = _dbContext.AttributeTypes.FirstOrDefault(at => at.Type == attribute.Type);
            if (attributeType == null)
            {
                // attributeType does not exist in database, create it
                attributeType = new AttributeType
                {
                    Type = attribute.Type
                };
                _dbContext.AttributeTypes.Add(attributeType);
            }
            attributeEntities.Add(new Attribute
            {
                Id = attribute.Id,
                Value = attribute.Value,
                AttributeType = attributeType
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
        var mushroom = _dbContext.Mushrooms
        .Include(m => m.Attributes)
            .ThenInclude(a => a.AttributeType)
        .Include(m => m.Attributes)
            .ThenInclude(a => a.RegisteredBy)
        .FirstOrDefault(m => m.Id == mushroomId);

        if (mushroom == null)
        {
            throw new Exception("Mushroom does not exist in database");
            return false;
        }

        var user = _dbContext.Users.FirstOrDefault(u => u.EmailAddress == researcherEmailAddress);

        var attributeEntities = new List<Attribute>();
        foreach (var attribute in inputModel.Entries)
        {
            // find attributeType by name based on attributeDto's Type
            var attributeType = _dbContext.AttributeTypes.FirstOrDefault(at => at.Type == attribute.Key);
            if (attributeType == null)
            {
                // attributeType does not exist in database, create it
                attributeType = new AttributeType
                {
                    Type = attribute.Key
                };
                _dbContext.AttributeTypes.Add(attributeType);
            }
            attributeEntities.Add(new Attribute
            {
                Value = attribute.Value,
                AttributeType = attributeType,
                RegisteredBy = user
            });
        }

        // add attribute entries
        mushroom.Attributes.Concat(attributeEntities);
        return true;
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
                RegisteredBy = a.RegisteredBy?.EmailAddress, // Some attributes don't have someone registered to them
                RegistrationDate = a.RegisteredBy?.RegistrationDate
            })
        };
        return mushroomDto;
    }

    public (int totalPages, IEnumerable<MushroomDto> mushrooms) GetMushroomsByCriteria(
        string? name, 
        int? stemSizeMinimum, 
        int? stemSizeMaximum, 
        int? capSizeMinimum, 
        int? capSizeMaximum, 
        string? color, 
        int pageSize, 
        int pageNumber)
    {
        // Start with a base query
        var query = _dbContext.Mushrooms
        .Include(m => m.Attributes)
            .ThenInclude(a => a.AttributeType)
        .Include(m => m.Attributes)
            .ThenInclude(a => a.RegisteredBy)
        .AsQueryable();

        // Apply filters
        if (!string.IsNullOrEmpty(name))
        {
            query = query.Where(m => m.Name.Contains(name));
        }

        if (stemSizeMinimum.HasValue)
        {
            // TODO: Get average of stem size and compare it to that
           query = query
            .Where(m => m.Attributes
                .Where(a => a.AttributeType.Type == "Stem Size")
                .Select(a => new { size = a.Value != null ? (int?)int.Parse(a.Value) : null })
                .Average(a => a.size) >= stemSizeMinimum.Value);
        }

        if (stemSizeMaximum.HasValue)
        {
            // TODO: Get average of stem size and compare it to that
            query = query
            .Where(m => m.Attributes
                .Where(a => a.AttributeType.Type == "Stem Size")
                .Select(a => new { size = a.Value != null ? (int?)int.Parse(a.Value) : null })
                .Average(a => a.size) <= stemSizeMaximum.Value);
        }

        if (capSizeMinimum.HasValue)
        {
            // TODO: Get average of cap size and compare it to that
            query = query
            .Where(m => m.Attributes
                .Where(a => a.AttributeType.Type == "Cap Size")
                .Select(a => new { size = a.Value != null ? (int?)int.Parse(a.Value) : null })
                .Average(a => a.size) >= capSizeMinimum.Value);
        }

        if (capSizeMaximum.HasValue)
        {
            // TODO: Get average of cap size and compare it to that
            query = query
            .Where(m => m.Attributes
                .Where(a => a.AttributeType.Type == "Cap Size")
                .Select(a => new { size = a.Value != null ? (int?)int.Parse(a.Value) : null })
                .Average(a => a.size) <= capSizeMaximum.Value);
        }


        if (!string.IsNullOrEmpty(color))
        {
            query = query.Where(m => m.Attributes.Any(a => a.AttributeType.Type == "Color" && a.Value == color));
        }

        int totalItemCount = query.Count();
        int totalPages = (int)Math.Ceiling(totalItemCount / (double)pageSize);

        // Apply paging
        var mushrooms = query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(m => new MushroomDto
            {
                Id = m.Id,
                Name = m.Name,
                Description = m.Description,
            })
            .ToList(); // Execute the query and convert to list

        // Return the result as a tuple
        return (totalPages, mushrooms);
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