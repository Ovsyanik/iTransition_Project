using System;
using System.Collections.Generic;

namespace project.Models.Entities
{
    public class Collection
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public TypeItem Type { get; set; }

        public string PathImage { get; set; }

        public User User { get; set; }

        public virtual ICollection<CustomField> Fieds { get; set; }

        public Collection() { }

        public Collection(string name, string description, TypeItem type, string pathImage, User user)
        {
            Name = name;
            Description = description;
            Type = type;
            PathImage = pathImage;
            User = user;
        }
    }
}
