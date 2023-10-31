using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using ShroomCity.Models.Dtos;
using ShroomCity.Models.InputModels;
using ShroomCity.Repositories.Interfaces;

namespace ShroomCity.Repositories.Implementations;

public class ResearcherRepository : IResearcherRepository
{
    private readonly ShroomCityDbContext _dbContext;

    public ResearcherRepository(ShroomCityDbContext dbContext)
    {
        _dbContext = dbContext;
    }


    public async Task<int?> CreateResearcher(string createdBy, ResearcherInputModel inputModel)
    {
        // find user with this email address, add the researcher role to them
        var user = _dbContext.Users.Include(u => u.roles).FirstOrDefault(u => u.EmailAddress == inputModel.EmailAddress);
        if (user == null)
        {
            throw new Exception("No user with this email address found.");
        }
        var researcherRole = _dbContext.Roles.FirstOrDefault(r => r.Name == "Researcher");
        if (researcherRole == null)
        {
            throw new Exception("Researcher role not found in database.");
        }
        user.roles.Add(researcherRole);
        _dbContext.SaveChanges();
        return user.Id;
    }

    public async Task<IEnumerable<ResearcherDto>?> GetAllResearchers()
    {
        var researcherRole = _dbContext.Roles.FirstOrDefault(r => r.Name == "Researcher");
        if (researcherRole == null)
        {
            throw new Exception("Researcher role not found in database.");
        }
        // get all users with the researcher role
        var researchers = _dbContext.Users.Include(u => u.roles).Where(u => u.roles.Contains(researcherRole));
        
        var researcherDtos = researchers.Select(r => new ResearcherDto
        {
            Id = r.Id,
            Name = r.Name,
            EmailAddress = r.EmailAddress,
            Bio = r.Bio,
            // TODO: get the mushrooms associated with that person specifically somehow? Despite the mushrooms not having a field for that?
            AssociatedMushrooms = _dbContext.Mushrooms.Include(m => m.Attributes).Select(m => new MushroomDto
            {
                Id = m.Id,
                Name = m.Name,
                Description = m.Description
            })
        });
        return researcherDtos;
    }

    public async Task<ResearcherDto?> GetResearcherByEmailAddress(string emailAddress)
    {
        var researcherRole = _dbContext.Roles.FirstOrDefault(r => r.Name == "Researcher");
        if (researcherRole == null)
        {
            throw new Exception("Researcher role not found in database.");
        }
        // Find user with this email address, check if they have the researcher role, return them if they do
        var user = _dbContext.Users.Include(u => u.roles).Where(u => u.roles.Contains(researcherRole)).FirstOrDefault(u => u.EmailAddress == emailAddress);
        if (user == null)
        {
            throw new Exception("No researcher with this email address found.");
        }
        var researcherDto = new ResearcherDto
        {
            Id = user.Id,
            Name = user.Name,
            EmailAddress = user.EmailAddress,
            Bio = user.Bio,
            AssociatedMushrooms = _dbContext.Mushrooms.Include(m => m.Attributes).Select(m => new MushroomDto
            {
                Id = m.Id,
                Name = m.Name,
                Description = m.Description
            })
        };
        return researcherDto;
    }

    public async Task<ResearcherDto?> GetResearcherById(int id)
    {
        // find user with this ID, see if they are a researcher, return them if they do
        var researcherRole = _dbContext.Roles.FirstOrDefault(r => r.Name == "Researcher");
        if (researcherRole == null)
        {
            throw new Exception("Researcher role not found in database.");
        }
        // Find user with this email address, check if they have the researcher role, return them if they do
        var user = _dbContext.Users.Include(u => u.roles).Where(u => u.roles.Contains(researcherRole)).FirstOrDefault(u => u.Id == id);
        if (user == null)
        {
            throw new Exception("No researcher with this ID found.");
        }
        var researcherDto = new ResearcherDto
        {
            Id = user.Id,
            Name = user.Name,
            EmailAddress = user.EmailAddress,
            Bio = user.Bio,
            AssociatedMushrooms = _dbContext.Mushrooms.Include(m => m.Attributes).Select(m => new MushroomDto
            {
                Id = m.Id,
                Name = m.Name,
                Description = m.Description
            })
        };
        return researcherDto;
    }
}