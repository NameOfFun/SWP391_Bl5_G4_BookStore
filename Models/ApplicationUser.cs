using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace BookStore.Models
{
    public class ApplicationUser : IdentityUser
    {
        [MaxLength(100)]
        public string? Name { get; set; }
        [MaxLength(500)]
        public string? Address { get; set; }
        public string? Avatar { get; set; }  
        public DateTime? CreatedAt { get; set; }
        public bool Status { get; set; } = true;
    }
}
