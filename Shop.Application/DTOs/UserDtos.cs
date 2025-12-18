namespace Shop.Application.DTOs;

public record RegisterDto(string Email, string Password, string DisplayName);
public record LoginDto(string Email, string Password);
public record UserDto(string Email, string DisplayName, string Token);