using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Bogus.DataSets;
using Microsoft.AspNetCore.Identity;

namespace ASP_RazorWeb.Models {
    public class AppUser: IdentityUser {
        
        [Column(TypeName = "nvarchar")]
        [StringLength(200)]
        public string? HomeAddress { get; set; }

        [Column(TypeName = "date")]
        public DateTime? DateOfBirth { get; set; }
    }
}