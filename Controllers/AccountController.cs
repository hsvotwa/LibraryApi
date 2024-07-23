using LibraryApi.DTOs;
using LibraryApi.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LibraryApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccountController(IAccountService accountService) : ControllerBase
{
    private readonly IAccountService _accountService = accountService;

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterModel model)
    {
        IdentityResult result = await _accountService.RegisterAsync(model);
        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }
        return Ok();
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {
        TokenModel? token = await _accountService.LoginAsync(model);

        if (token == null)
        {
            return Unauthorized();
        }

        return Ok(token);
    }
}
