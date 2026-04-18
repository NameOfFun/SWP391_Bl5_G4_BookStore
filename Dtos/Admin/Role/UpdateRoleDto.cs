using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace BookStore.Dtos.Admin.Role;

public class UpdateRoleDto
{
    [Required]
    public string Id { get; set; } = null!;

    [ValidateNever]
    public string Name { get; set; } = null!; // display only, not editable

    [MaxLength(500, ErrorMessage = "Mô tả tối đa 500 ký tự")]
    public string? Description { get; set; }

    public bool IsActive { get; set; }
}
