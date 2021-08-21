using Microsoft.AspNetCore.Identity;
using System;

namespace project.Models.Entities
{
    public class User : IdentityUser
    {
        public RoleUser Role { get; set; }

        public string Provider { get; set; }

        private static User instance;

        private User(string email, RoleUser role) 
        {
            Email = email;
            Role = role;
        }

        public User()
        { }

        public static User GetInstance()
        {
            return instance;
        }

        public static User GetInstance(string email, RoleUser role)
        {
            if (instance == null)
                instance = new User(email, role);
            return instance;
        }

        public static void SignOut()
        {
            instance = null;
        }
    }
}
