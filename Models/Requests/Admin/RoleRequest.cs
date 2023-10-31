
using System.ComponentModel.DataAnnotations;

namespace Models.Requests.Admin
{
    public class RoleRequest
    {
        [Required]
        public string Name { get; set; }
    }
}
