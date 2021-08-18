using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace project.Models.Entities
{
    public class Comment
    {
        public int Id { get; set; }

        public string Text { get; set; }

        public string Author { get; set; }

        public DateTime CreatedDate { get; set; }

        public int ItemId { get; set; }

        public Item Item { get; set; }
    }
}
