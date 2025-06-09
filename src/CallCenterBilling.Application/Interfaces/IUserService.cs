using CallCenterBilling.Application.DTOs;

namespace CallCenterBilling.Application.Interfaces;

public interface IUserService
{
    Task<UserDto?> GetUserByIdAsync(string id);
    Task<UserDto?> GetUserByEmailAsync(string email);
    Task<IEnumerable<UserDto>> GetAllUsersAsync();
    Task<(bool Success, string[] Errors)> RegisterUserAsync(RegisterDto registerDto);
    Task<bool> UpdateUserAsync(string id, UserDto userDto);
    Task<bool> DeleteUserAsync(string id);
    Task<(bool Success, string Error)> LoginAsync(LoginDto loginDto);
    Task LogoutAsync();
}