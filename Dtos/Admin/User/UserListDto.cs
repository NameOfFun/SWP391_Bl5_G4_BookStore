namespace BookStore.Dtos.Admin.User;

public class UserListDto
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public bool IsActive { get; set; }
    public bool IsLockedOut { get; set; }
    public IList<string> Roles { get; set; } = new List<string>();
    public string RolesDisplay => Roles.Count > 0 ? string.Join(", ", Roles) : "—";
}
