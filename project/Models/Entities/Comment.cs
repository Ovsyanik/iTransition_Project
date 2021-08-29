using System;
using System.ComponentModel.DataAnnotations;

namespace project.Models.Entities
{
    public class Comment
    {
        public int Id { get; set; }

        [MaxLength(100)]
        public string Text { get; set; }

        public string Author { get; set; }

        public DateTime CreatedDate { get; set; }

        public int ItemId { get; set; }

        public Item Item { get; set; }
    }
}
