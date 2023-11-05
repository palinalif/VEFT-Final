using System.Diagnostics;
using System.Security.Claims;
using System.Security.Principal;
using Microsoft.AspNetCore.Mvc;
using Mono.TextTemplating;
using ShroomCity.Models.InputModels;
using ShroomCity.Services.Implementations;
using ShroomCity.Services.Interfaces;

namespace ShroomCity.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly IAccountService _accountService;
    private readonly ITokenService _tokenService;

    public AccountController(IAccountService accountService, ITokenService tokenService)
    {
        _accountService = accountService;
        _tokenService = tokenService;
    }

    [HttpPost]
    [Route("register")]
    public async Task<IActionResult> RegisterAsync([FromBody] RegisterInputModel inputModel)
    {
        var newUserDto = await _accountService.Register(inputModel);
        if (newUserDto == null)
        {
            return StatusCode(StatusCodes.Status401Unauthorized); // Something went wrong when creating a user
        }
        string token = await _tokenService.GenerateJwtToken(newUserDto);
        return Ok(token);
    }

    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> LoginAsync([FromBody] LoginInputModel inputModel)
    {
        var userDto = await _accountService.SignIn(inputModel);
        string token = await _tokenService.GenerateJwtToken(userDto);
        return Ok(token);
    }

    [HttpPost]
    [Route("logout")]
    public async Task<IActionResult> LogoutAsync()
    {
        var tokenId = User.Claims.FirstOrDefault(c => c.Type == "TokenId")?.Value;
        if (tokenId == null)
        {
            // No one is logged in
            return StatusCode(StatusCodes.Status401Unauthorized);
        }
        var intTokenId = Int32.Parse(tokenId);
        await _accountService.SignOut(intTokenId);
        return Ok();
    }

    [HttpPost]
    [Route("profile")]
    public async Task<IActionResult> GetProfileInformation()
    {
        if (User.Claims.Any())
        {
            // This gives an error
            return Ok(User.Claims);
        }
        return StatusCode(StatusCodes.Status401Unauthorized);
    }
}
