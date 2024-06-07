using Microsoft.EntityFrameworkCore;
using project.Models.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace project.Models.Repositories
{
    public class TagRepository : ITagRepository
    {
        private readonly ApplicationContext _db;

        public TagRepository(ApplicationContext db)
        {
            _db = db;
        }


        public async Task<List<Tags>> GetAllAsync()
        {
            return await _db.Tags.ToListAsync();
        }


        public async Task<List<string>> GetAllValuesAsync()
        {
            var tags = await GetAllAsync();

            return tags.Select(tag => tag.Value).ToList();
        }


        public async Task<Tags> AddAsync(Tags tag)
        {
            await _db.Tags.AddAsync(tag);
            await SaveAsync();
            return tag;
        }


        public async Task SaveAsync()
        {
            await _db.SaveChangesAsync();
        }
    }
}
