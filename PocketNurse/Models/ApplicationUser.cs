using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace PocketNurse.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        [StringLength(32)]
        public string State { get; set; }
        [StringLength(32)]
        public string Area { get; set; }
    }
}
