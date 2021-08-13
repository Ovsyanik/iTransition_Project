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

        public List<Tags> Tags { get; set; }

        public int CustomFieldId { get; set; } 

        public string CustomFieldValue { get; set; }

        public virtual ICollection<CustomField> Fields { get; set; }
    }
}
