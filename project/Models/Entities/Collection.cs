using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace project.Models.Entities
{
    public class Collection
    {
        public int Id { get; set; }

        public string Name { get; set; }

        [MaxLength(200)]
        public string Description { get; set; }

        public string PathImage { get; set; }

        public IdentityUser User { get; set; }

        public TypeItem Type { get; set; }

        public virtual ICollection<Item> Items { get; set; }

        public virtual ICollection<CustomField> Fields { get; set; }
    }
}
