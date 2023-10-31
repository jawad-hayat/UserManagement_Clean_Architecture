
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    public class User : IdentityUser
    {
        [Required]
        [StringLength(60, MinimumLength = 3)]
        public string FirstName { get; set; }
        [Required]
        [StringLength(60, MinimumLength = 3)]
        public string LastName { get; set; }

        public bool? IsActive { get; set; }
        public bool? IsDeleted { get; set; }

        [DataType(DataType.Date)]
        public DateTime? CreatedDate { get; set; }

        //For refresh token
        public List<RefreshToken>? RefreshTokens { get; set; }
    }
}
