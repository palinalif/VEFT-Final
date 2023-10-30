using ShroomCity.Repositories.Interfaces;

namespace ShroomCity.Repositories.Implementations;

public class TokenRepository : ITokenRepository
{
    public Task BlacklistToken(int tokenId)
    {
        throw new NotImplementedException();
    }

    public Task<int> AddToken(string token)
    {
        throw new NotImplementedException();
    }

    public Task<bool> IsTokenBlacklisted(int tokenId)
    {
        throw new NotImplementedException();
    }
}