using Microsoft.AspNetCore.Identity;

namespace Shop.Domain.Entities;

public class AppUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
}