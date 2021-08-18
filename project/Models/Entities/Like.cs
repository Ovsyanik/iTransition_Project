using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace project.Models.Entities
{
    public class Like
    {
        public int Id { get; set; }

        public string UserName { get; set; }

        public int ItemId { get; set; }
        
        public Item Item { get; set; }
    }
}
