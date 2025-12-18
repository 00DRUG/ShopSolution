using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Shop.Domain.Entities;
using Shop.Application.DTOs;
using Shop.Application.Interfaces;
using Asp.Versioning;

namespace Shop.Api.Controllers;

[ApiController]
// For both versions
[ApiVersion("1.0")]
[ApiVersion("2.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class AccountController : ControllerBase
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly ITokenService _tokenService;

    public AccountController(UserManager<AppUser> userManager,
        SignInManager<AppUser> signInManager,
        ITokenService tokenService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<ProductDto>> Register(RegisterDto registerDto)
    {
        // Check if email is already taken
        if (await _userManager.FindByEmailAsync(registerDto.Email) != null)
            return BadRequest("Email is already taken");

        var user = new AppUser
        {
            FullName = registerDto.DisplayName,
            Email = registerDto.Email,
            UserName = registerDto.Email
        };

        var result = await _userManager.CreateAsync(user, registerDto.Password);

        if (!result.Succeeded) return BadRequest(result.Errors);

        // Return the user data along with the token
        return Ok(new UserDto(
            user.Email,
            user.FullName,
            _tokenService.CreateToken(user)
        ));
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
    {
        var user = await _userManager.FindByEmailAsync(loginDto.Email);

        if (user == null) return Unauthorized("Invalid password or username");

        var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

        if (!result.Succeeded) return Unauthorized("Autorization Error");

        return Ok(new UserDto(
            user.Email,
            user.FullName,
            _tokenService.CreateToken(user)
        ));
    }
}