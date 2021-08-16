using project.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace project.Models.Repositories
{
    public class TagRepository
    {
        private readonly MyDbContext _context;

        public TagRepository(MyDbContext myDbContext)
        {
            _context = myDbContext;
        }

        public Tags Add(Tags tag)
        {
            _context.Tags.Add(tag);
            Save();
            return tag;
        }

        private void Save()
        {
            _context.SaveChanges();
        }
    }
}
