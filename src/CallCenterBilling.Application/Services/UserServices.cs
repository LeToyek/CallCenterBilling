using AutoMapper;
using Microsoft.AspNetCore.Identity;
using CallCenterBilling.Application.DTOs;
using CallCenterBilling.Application.Interfaces;
using CallCenterBilling.Domain.Entities;

namespace CallCenterBilling.Application.Services;

public class UserService : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IMapper _mapper;

    public UserService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IMapper mapper)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _mapper = mapper;
    }

    public async Task<UserDto?> GetUserByIdAsync(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        return user != null ? _mapper.Map<UserDto>(user) : null;
    }

    public async Task<UserDto?> GetUserByEmailAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        return user != null ? _mapper.Map<UserDto>(user) : null;
    }

    public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
    {
        var users = _userManager.Users.ToList();
        return _mapper.Map<IEnumerable<UserDto>>(users);
    }

    public async Task<(bool Success, string[] Errors)> RegisterUserAsync(RegisterDto registerDto)
    {
        var user = new ApplicationUser
        {
            UserName = registerDto.Email,
            Email = registerDto.Email,
            FirstName = registerDto.FirstName,
            LastName = registerDto.LastName,
            CreatedAt = DateTime.UtcNow
        };

        var result = await _userManager.CreateAsync(user, registerDto.Password);
        
        if (result.Succeeded)
        {
            return (true, Array.Empty<string>());
        }

        return (false, result.Errors.Select(e => e.Description).ToArray());
    }

    public async Task<bool> UpdateUserAsync(string id, UserDto userDto)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return false;

        user.FirstName = userDto.FirstName;
        user.LastName = userDto.LastName;
        user.IsActive = userDto.IsActive;
        user.UpdatedAt = DateTime.UtcNow;

        var result = await _userManager.UpdateAsync(user);
        return result.Succeeded;
    }

    public async Task<bool> DeleteUserAsync(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return false;

        var result = await _userManager.DeleteAsync(user);
        return result.Succeeded;
    }

    public async Task<(bool Success, string Error)> LoginAsync(LoginDto loginDto)
    {
        var result = await _signInManager.PasswordSignInAsync(
            loginDto.Email,
            loginDto.Password,
            loginDto.RememberMe,
            lockoutOnFailure: false);

        if (result.Succeeded)
            return (true, string.Empty);

        if (result.IsLockedOut)
            return (false, "Account is locked out.");

        if (result.RequiresTwoFactor)
            return (false, "Two-factor authentication required.");

        return (false, "Invalid email or password.");
    }

    public async Task LogoutAsync()
    {
        await _signInManager.SignOutAsync();
    }
}