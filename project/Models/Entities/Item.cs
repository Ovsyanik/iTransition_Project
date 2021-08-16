using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace project.Models.Entities
{
    public class Item
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public int CollectionId { get; set; }

        public virtual ICollection<CustomField> CustomFieldId { get; set; }

        public virtual List<CustomFieldValue> CustomFieldValues { get; set; }

        public virtual ICollection<Tags> Tags { get; set; }
    }
}
