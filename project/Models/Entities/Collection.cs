using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace project.Models.Entities
{
    public class Collection<Item>
    {
        public int Id { get; set; }

        public string Description { get; set; }

        public TypeItem Type { get; set; }

        public List<Item> Items { get; set; }
    }
}
