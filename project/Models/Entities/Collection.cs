using System;
using System.Collections.Generic;

namespace project.Models.Entities
{
    public class Collection
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string PathImage { get; set; }

        public User User { get; set; }

        public TypeItem Type { get; set; }

        public virtual ICollection<Item> Items { get; set; }

        public virtual ICollection<CustomField> Fields { get; set; }
    }
}
