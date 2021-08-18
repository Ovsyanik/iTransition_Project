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

        public async Task<Tags> AddAsync(Tags tag)
        {
            await _context.Tags.AddAsync(tag);
            await SaveAsync();
            return tag;
        }

        private async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
