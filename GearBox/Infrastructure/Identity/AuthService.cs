using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Core.DTOs;
using Core.Exceptions;
using Core.Interfaces.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Identity;

public class AuthService(
    UserManager<AppUser> userManager,
    IConfiguration config) : IAuthService
{
    public async Task<UserDto> LoginAsync(LoginDto dto)
    {
        var user = await userManager.FindByEmailAsync(dto.Email)
            ?? throw new UnauthorizedException("Invalid email or password");

        var valid = await userManager.CheckPasswordAsync(user, dto.Password);
        if (!valid) throw new UnauthorizedException("Invalid email or password");

        return await BuildUserDtoAsync(user);
    }

    public async Task<UserDto> RegisterAsync(RegisterDto dto)
    {
        var user = new AppUser
        {
            FirstName = dto.FirstName,
            LastName  = dto.LastName,
            Email     = dto.Email,
            UserName  = dto.Email,
        };

        var result = await userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
            throw new BadRequestException(string.Join("; ", result.Errors.Select(e => e.Description)));

        await userManager.AddToRoleAsync(user, "Customer");

        return await BuildUserDtoAsync(user);
    }

    public async Task<UserDto?> GetCurrentUserAsync(string email)
    {
        var user = await userManager.FindByEmailAsync(email);
        return user is null ? null : await BuildUserDtoAsync(user);
    }

    private async Task<UserDto> BuildUserDtoAsync(AppUser user)
    {
        var roles = await userManager.GetRolesAsync(user);
        return new UserDto
        {
            Email     = user.Email!,
            FirstName = user.FirstName,
            LastName  = user.LastName,
            Token     = GenerateToken(user, roles),
            Roles     = roles,
        };
    }

    private string GenerateToken(AppUser user, IList<string> roles)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.Email,          user.Email!),
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.GivenName,      user.FirstName),
            new(ClaimTypes.Surname,        user.LastName),
        };

        foreach (var role in roles)
            claims.Add(new Claim(ClaimTypes.Role, role));

        var key   = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JwtSettings:SecretKey"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer:             config["JwtSettings:Issuer"],
            audience:           config["JwtSettings:Audience"],
            claims:             claims,
            expires:            DateTime.UtcNow.AddMinutes(Convert.ToDouble(config["JwtSettings:ExpiryMinutes"])),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
