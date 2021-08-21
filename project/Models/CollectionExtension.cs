using project.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace project.Models
{
    public static class CollectionExtension
    {
        public static string ListToString(this ICollection<Tags> tags)
        {
            StringBuilder builder = new StringBuilder();
            if (tags != null)
            {
                tags.ToList().ForEach(tag => builder.Append(tag.Value + ", "));
                builder.Remove(builder.Length - 2, 2);
            }
            return builder.ToString();
        }
    }
}
