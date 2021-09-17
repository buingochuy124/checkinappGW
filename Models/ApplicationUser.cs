using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CheckInGWDN.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Display(Name = "Full Name")]
        public string FullName { get; set; }
        [NotMapped]
        public string Role { get; set; }
    }
}