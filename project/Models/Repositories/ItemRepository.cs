using project.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace project.Models.Repositories
{
    public class ItemRepository
    {
        private readonly MyDbContext _context;

        public ItemRepository(MyDbContext context)
        {
            _context = context;
        }

        public List<Item> GetAll(int id)
        {
            Collection collection = _context.Collections.Where(col => col.Id == id).FirstOrDefault();

            return collection.Items;
        }

        public async Task Add(Item item)
        {

            await Save();
        }

        public async Task Change(Item item)
        {

            await Save();
        }

        public async Task Delete(Item item)
        {
            await Save();
        }

        private async Task Save()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<List<Collection>> GetCollections()
        {
            return new List<Collection>();   
        }


    }
}
