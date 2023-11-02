using System.Diagnostics;
using System.Security.Principal;
using Microsoft.AspNetCore.Mvc;
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
            return StatusCode(StatusCodes.Status500InternalServerError); // Something went wrong when creating a user
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
        // get id of currently logged in user somehow
        int tokenId = 1; // so there aren't errors, change later
        await _accountService.SignOut(tokenId);
        return Ok();
    }

    [HttpPost]
    [Route("profile")]
    public async Task<IActionResult> GetProfileInformation()
    {
        if (User.Claims != null)
        {
            return Ok(User.Claims);
        }
        return Ok(); // TODO: Replace with appropriate status code
    }
}
