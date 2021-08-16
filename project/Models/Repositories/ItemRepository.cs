using Microsoft.EntityFrameworkCore;
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

        public async Task<List<Item>> GetAllAsync(int id)
        {
            List<Item> items = await _context.Items.Where(item => item.CollectionId == id).Include(item => item.CustomFieldValues).ToListAsync();

            return items;
        }

        public async Task<Guid> AddAsync(Item item)
        {
            await _context.AddAsync(item);
            await SaveAsync();

            return item.Id;
        }

        public async Task Change(Item item)
        {

            await SaveAsync();
        }

        public async Task Delete(Item item)
        {
            await SaveAsync();
        }

        private async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<List<Collection>> GetCollections()
        {
            return new List<Collection>(); 
        }


    }
}
