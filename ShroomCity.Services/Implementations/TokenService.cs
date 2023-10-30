using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ShroomCity.Models.Dtos;
using ShroomCity.Repositories.Interfaces;
using ShroomCity.Services.Interfaces;

namespace ShroomCity.Services.Implementations;

public class JwtConfiguration
{
    /// <summary>
    /// The secret used to sign the JWT token
    /// </summary>
    public string Secret { get; set; } = "";
    /// <summary>
    /// Expiration in minutes for the JWT token.
    /// </summary>
    public string ExpirationInMinutes { get; set; } = "";
    /// <summary>
    /// The issuer of the JWT token. If the issuer is not a known enity, the JWT token should be rejected. In our
    /// example this API is the issuer - but that is not always the case.
    /// </summary>
    public string Issuer { get; set; } = "";
    /// <summary>
    /// The audience of the token. The services which are expected to receive and use the token. In our example
    /// this API is the audience - but that is not always the case.
    /// </summary>
    public string Audience { get; set; } = "";
}

public class TokenService : ITokenService
{
    private readonly string _signingKey;
    private readonly string _issuer;
    private readonly string _audience;
    private readonly int _expirationInMinutes;
    private readonly ITokenRepository _tokenRepository;
    public TokenService(ITokenRepository tokenRepository, IConfiguration configuration)
    {
        _signingKey = configuration["TokenAuthentication:Key"] ?? "";
        _issuer = configuration["TokenAuthentication:Issuer"] ?? "";
        _audience = configuration["TokenAuthentication:Audience"] ?? "";
        if (int.TryParse(configuration["TokenAuthentication:DurationInMinutes"], out int expiration))
        {
            _expirationInMinutes = expiration;
        }
        else
        {
            _expirationInMinutes = 120; // Default value
        }
        _tokenRepository = tokenRepository;
    }

    public async Task BlacklistToken(int tokenId)
    {
        await _tokenRepository.BlacklistToken(tokenId);
    }


    public string GenerateJwtToken(UserDto user)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, user.Email),
            new("FullName", user.Name)
        };

        var claimsIdentity = new ClaimsIdentity(claims, JwtBearerDefaults.AuthenticationScheme);
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_signingKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(_issuer, _audience, claimsPrincipal.Claims, expires: DateTime.Now.AddMinutes(_expirationInMinutes), signingCredentials: credentials);
        var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);
        _tokenRepository.AddToken(jwtToken);
        return jwtToken;
    }

    public async Task<bool> IsTokenBlacklisted(int tokenId)
    {
        return await _tokenRepository.IsTokenBlacklisted(tokenId);
    }
}