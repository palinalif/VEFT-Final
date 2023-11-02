using Microsoft.VisualBasic;
using ShroomCity.Models.Entities;
using ShroomCity.Repositories.Interfaces;

namespace ShroomCity.Repositories.Implementations;

public class TokenRepository : ITokenRepository
{
    private readonly ShroomCityDbContext _dbContext;

    public TokenRepository(ShroomCityDbContext dbContext)
    {
        _dbContext = dbContext;
    }


    public async Task BlacklistToken(int tokenId)
    {
        var token = _dbContext.JwtTokens.FirstOrDefault(t => t.Id == tokenId);
        if (token == null)
        {
            throw new Exception("Token ID does not exist in database");
        }
        token.Blacklisted = true;
        _dbContext.SaveChanges();
    }

    public async Task<int> CreateToken()
    {
        var entity = new JwtToken
        {
            Blacklisted = false
        };
        _dbContext.JwtTokens.Add(entity);
        _dbContext.SaveChanges();
        return entity.Id;
    }

    public async Task<bool> IsTokenBlacklisted(int tokenId)
    {
        var token = _dbContext.JwtTokens.FirstOrDefault(t => t.Id == tokenId);
        if (token == null)
        {
            throw new Exception("Token ID does not exist in database");
        }
        return token.Blacklisted;
    }
}