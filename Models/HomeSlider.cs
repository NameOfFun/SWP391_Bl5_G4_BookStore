using Microsoft.AspNetCore.Identity;

namespace BookStore.Models;

public class HomeSlider
{
    public int HomeSliderId { get; set; }

    public string ImageUrl { get; set; } = null!;

    public string? Caption { get; set; }

    public string? LinkUrl { get; set; }

    public int DisplayOrder { get; set; }

    public bool IsActive { get; set; } = true;

    /// <summary>User tạo / cập nhật banner (nullable).</summary>
    public string? CreatedByUserId { get; set; }

    /// <summary>Optional role link (audience or CMS scope).</summary>
    public string? RoleId { get; set; }

    public virtual IdentityUser? CreatedBy { get; set; }

    public virtual IdentityRole? Role { get; set; }
}
