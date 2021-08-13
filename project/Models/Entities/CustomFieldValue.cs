using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace project.Models.Entities
{
    public class CustomFieldValue : CustomField
    {
        public int Id { get; set; }

        public int CollectionId { get; set; }

        public int customField { get; set; } 

        public int Value { get; set; }
    }
}
