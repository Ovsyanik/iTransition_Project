using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace project.Models.Entities
{
    public class Item
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public List<Tags> Tags { get; set; }

        public int Visible { get; set; }

        public int integer1 { get; set; }

        public int integer2 { get; set; }

        public int integer3 { get; set; }

        public string field1 { get; set; }

        public string field2 { get; set; }

        public string field3 { get; set; }

        public string textField1 { get; set; }

        public string textField2 { get; set; }

        public string textField3 { get; set; }
    }
}
