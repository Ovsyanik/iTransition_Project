using project.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace project.Models.Repositories
{
    public class CollectionRepository
    {
        private readonly MyDbContext _context;

        public CollectionRepository(MyDbContext context)
        {
            _context = context;
        }

        public List<Collection> GetAllByUser(User user)
        {
            return _context.Collections.Where(col => col.User.Id == user.Id).ToList();
        }

        public async Task<int> Add(Collection collection)
        {
            await _context.Collections.AddAsync(collection);
            await Save();
            return collection.Id;
        }

        private async Task Save()
        {
            await _context.SaveChangesAsync();
        }


    }
}
