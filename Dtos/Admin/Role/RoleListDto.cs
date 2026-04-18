namespace BookStore.Dtos.Admin.Role;

public class RoleListDto
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public bool IsSystemRole { get; set; }
    public int UserCount { get; set; }
}
