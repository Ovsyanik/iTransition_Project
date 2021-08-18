using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace project.Models.Entities
{
    public class Item
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int CollectionId { get; set; }

        public Collection Collection { get; set; }

        public virtual ICollection<CustomField> CustomFields { get; set; }

        public virtual ICollection<CustomFieldValue> CustomFieldValues { get; set; }

        public virtual ICollection<Tags> Tags { get; set; }

        public virtual ICollection<Comment> Comments { get; set; }

        public virtual ICollection<Like> Likes { get; set; }
    }
}
