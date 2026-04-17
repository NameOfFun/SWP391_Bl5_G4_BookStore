using Microsoft.AspNetCore.Identity;

namespace BookStore.Models;

public class ApplicationUser : IdentityUser
{
    public string Name { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;
}
