using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace project.Models.Entities
{
    public class Item
    {
        public int Id { get; set; }

        [MaxLength(50)]
        [NotNull]
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
