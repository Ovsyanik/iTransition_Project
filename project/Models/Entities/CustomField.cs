using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace project.Models.Entities
{
    public class CustomField
    {
        public int Id { get; set; }

        public string Title { get; set; }
        
        public int CollectionId { get; set; }

        public Collection Collection { get; set; }

        public CustomFieldType CustomFieldType { get; set; }
    }
}
