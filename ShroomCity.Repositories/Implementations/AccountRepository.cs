using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ShroomCity.Models.Dtos;
using ShroomCity.Models.Entities;
using ShroomCity.Models.InputModels;
using ShroomCity.Repositories.Interfaces;
using ShroomCity.Utilities.Hasher;

namespace ShroomCity.Repositories.Implementations;

public class AccountRepository : IAccountRepository
{
    private readonly ShroomCityDbContext _dbContext;
    private readonly string _salt;

    public AccountRepository(ShroomCityDbContext dbContext, IConfiguration configuration)
    {
        _dbContext = dbContext;
        _salt = configuration["TokenAuthentication:Salt"] ?? "";
    }


    public async Task<UserDto?> Register(RegisterInputModel inputModel)
    {
        var hashedPassword = Hasher.HashPassword(inputModel.Password, _salt);
        var defaultRole = _dbContext.Roles.Include(r => r.Permissions).FirstOrDefault(r => r.Name == "Analyst");
        if (defaultRole == null)
        {
            throw new Exception("Analyst role not found");
        }
        if (_dbContext.Users.Any(u => u.EmailAddress == inputModel.EmailAddress))
        {
            // user already exists
            return null;
        }

        var entity = new User 
        {
            Name = inputModel.FullName,
            EmailAddress = inputModel.EmailAddress,
            HashedPassword = hashedPassword,
            RegistrationDate = DateTime.UtcNow,
            Bio = inputModel.Bio,
            roles = new List<Role>()
        };
        entity.roles.Add(defaultRole);
        _dbContext.Users.Add(entity);
        _dbContext.SaveChanges();

        // convert the entity to a UserDto
        UserDto userDto = new UserDto
        {
            Id = entity.Id,
            Name = entity.Name,
            Email = entity.EmailAddress,
            Bio = inputModel.Bio,
            Permissions = defaultRole.Permissions.Select(p => p.Code)
        };
        return userDto;
    }

    public async Task<UserDto?> SignIn(LoginInputModel inputModel)
    {
        var hashedPassword = Hasher.HashPassword(inputModel.Password, _salt);
        var user = _dbContext.Users.Include(u => u.roles).ThenInclude(r => r.Permissions).FirstOrDefault(u => u.EmailAddress == inputModel.EmailAddress && u.HashedPassword == hashedPassword);
        if (user == null)
        {
            return null;
        }
        List<string> permissions = new List<string>();
        // Get the roles of the user
        foreach (var role in user.roles)
        {
            // get every permission the role has
            foreach (var perm in role.Permissions)
            {
                // if the permission does not already exist in the role, add it
                if (!permissions.Contains(perm.Code))
                {
                    permissions.Add(perm.Code);
                }
            }
        }

        // what to do with token ID??
        return new UserDto 
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.EmailAddress,
            Bio = user.Bio,
            Permissions = permissions
        };
    }
}