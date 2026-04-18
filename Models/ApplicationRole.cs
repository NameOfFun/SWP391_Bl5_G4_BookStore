using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace BookStore.Models
{
    public class ApplicationRole : IdentityRole
    {
        [MaxLength(500)]
        public string? Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsSystemRole { get; set; } = false;
        public bool Status { get; set; } = false;
        //public RoleStatus Status { get; set; } = RoleStatus.Active;
    }

    //public enum RoleStatus
    //{
    //    Active,
    //    Inactive
    //}
}
