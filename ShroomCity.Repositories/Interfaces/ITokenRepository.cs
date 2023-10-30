namespace ShroomCity.Repositories.Interfaces;

public interface ITokenRepository
{
    Task<int> AddToken(string token);
    Task BlacklistToken(int tokenId);
    Task<bool> IsTokenBlacklisted(int tokenId);
}