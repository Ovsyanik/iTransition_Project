using System.ComponentModel.DataAnnotations;

namespace project.Models.Entities
{
    public class CustomFieldValue
    {
        public int Id { get; set; }

        public int CustomField { get; set; }

        [MaxLength(50)]
        public string Value { get; set; }

        public int CollectionId { get; set; }

        public Item Item { get; set; }
    }
}
