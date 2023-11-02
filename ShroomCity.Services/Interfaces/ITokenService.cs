using ShroomCity.Models.Dtos;

namespace ShroomCity.Services.Interfaces;

public interface ITokenService
{
    Task<string> GenerateJwtToken(UserDto user);
    Task<bool> IsTokenBlacklisted(int tokenId);
    Task BlacklistToken(int tokenId);
}