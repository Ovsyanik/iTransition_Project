using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace project.Models.Entities
{
    public class User
    {
        public int Id { get; set; }
        
        public string FirstName { get; set; }
        
        public string LastName { get; set; }

        public RoleUser Role { get; set; }
        
        public string Email { get; set; }

        public string Password { get; set; }

        public User() { }

        public User(string firstName, string lastName, RoleUser role, string email, string password)
        {
            FirstName = firstName;
            LastName = lastName;
            Role = role;
            Email = email;
            Password = password;
        }

        public User(int id, string firstName, string lastName, RoleUser role, string email, string password)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            Role = role;
            Email = email;
            Password = password;
        }
    }
}
