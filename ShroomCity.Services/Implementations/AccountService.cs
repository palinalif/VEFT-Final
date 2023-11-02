using ShroomCity.Models.Dtos;
using ShroomCity.Models.InputModels;
using ShroomCity.Repositories.Interfaces;
using ShroomCity.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using ShroomCity.Utilities.Hasher;

namespace ShroomCity.Services.Implementations;

public class AccountService : IAccountService
{
    private readonly IAccountRepository _accountRepository;
    private readonly ITokenService _tokenService;

    public AccountService(IAccountRepository accountRepository, ITokenService tokenService)
    {
        _accountRepository = accountRepository;
        _tokenService = tokenService;
    }


    public async Task<UserDto?> Register(RegisterInputModel inputModel)
    {
        var user = await _accountRepository.Register(inputModel);
        if (user != null)
        {
            return user;
        }
        else
        {
            return null;
        }
    }

    public async Task<UserDto?> SignIn(LoginInputModel inputModel)
    {
        var user = await _accountRepository.SignIn(inputModel);
        if (user == null)
        {
            throw new InvalidDataException();
        }
        return user;
    }

    public async Task SignOut(int tokenId)
    {
        // Invalidate tokenId
        await _tokenService.BlacklistToken(tokenId);
    }
}