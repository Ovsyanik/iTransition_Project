using Microsoft.AspNetCore.Identity;

namespace project.Models.Entities
{
    public class User : IdentityUser
    {
        public RoleUser Role { get; set; }

        public string Provider { get; set; }
    }
}
