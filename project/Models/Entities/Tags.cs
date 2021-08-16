using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace project.Models.Entities
{
    public class Tags
    {
        public int Id { get; set; }

        public string Value { get; set; }

        public Item Item { get; set; }
    }
}
