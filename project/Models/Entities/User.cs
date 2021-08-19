using Microsoft.AspNetCore.Identity;

namespace project.Models.Entities
{
    public class User : IdentityUser
    {
        public RoleUser Role { get; set; }

        public string Provider { get; set; }

        private static User instance;

        private User()
        { }

        public static User getInstance()
        {
            if (instance == null)
                instance = new User();
            return instance;
        }
    }
}
