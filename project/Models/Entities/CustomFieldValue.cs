using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace project.Models.Entities
{
    public class CustomFieldValue
    {
        public int Id { get; set; }

        public int CustomField { get; set; } 

        public string Value { get; set; }

        public int CollectionId { get; set; }

        public Item Item { get; set; }
    }
}
