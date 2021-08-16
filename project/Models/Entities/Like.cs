using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace project.Models.Entities
{
    public class Like
    {
        public int Id { get; set; }

        public string Username { get; set; }

        public int ItemID { get; set; }
        
        public Item Item { get; set; }
    }
}
