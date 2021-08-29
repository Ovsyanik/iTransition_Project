using Microsoft.EntityFrameworkCore;
using project.Models.Entities;
using System.Collections.Generic;
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


        public async Task<List<Tags>> GetAllAsync()
        {
            return await _context.Tags.ToListAsync();
        }


        public async Task<List<string>> GetAllValuesAsync()
        {
            var tags = await GetAllAsync();
            List<string> tagsString = new List<string>();
            foreach(Tags tag in tags) {
                tagsString.Add(tag.Value);
            }
            return tagsString;
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
