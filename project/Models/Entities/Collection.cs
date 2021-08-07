using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace project.Models.Entities
{
    public class Collection
    {
        public int Id { get; set; }

        public string Description { get; set; }

        public List<Book> Books { get; set; }
    }
}
