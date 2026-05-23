using Core.DTOs;

namespace Core.Interfaces.Services;

public interface IAuthService
{
    Task<UserDto> LoginAsync(LoginDto dto);
    Task<UserDto> RegisterAsync(RegisterDto dto);
    Task<UserDto?> GetCurrentUserAsync(string email);
}
