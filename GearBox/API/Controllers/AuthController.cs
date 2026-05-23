using System.Security.Claims;
using Core.DTOs;
using Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(LoginDto dto)
        => Ok(await authService.LoginAsync(dto));

    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register(RegisterDto dto)
        => Ok(await authService.RegisterAsync(dto));

    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<UserDto>> GetCurrentUser()
    {
        var email = User.FindFirstValue(ClaimTypes.Email)!;
        var user  = await authService.GetCurrentUserAsync(email);
        return user is null ? Unauthorized() : Ok(user);
    }
}
