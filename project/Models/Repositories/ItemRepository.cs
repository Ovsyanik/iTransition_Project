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

        public async Task<List<Item>> GetLastItemsAsync()
        {
            return await _context.Items
                .Include(i => i.Likes)
                .OrderByDescending(i => i.Id).Take(5).ToListAsync();
        }

        public async Task<List<Item>> GetAllAsync(int id)
        {
            return await _context.Items
                .Where(item => item.CollectionId == id)
                .Include(item => item.CustomFieldValues)
                .Include(Item => Item.Tags).ToListAsync();
        }


        public async Task<List<Item>> SearchItems(string query)
        {
            return await _context.Items.Where(i => 
                EF.Functions.FreeText(i.Name, query) || 
                EF.Functions.FreeText(i.Collection.Description, query) || 
                i.Comments.Any(c => EF.Functions.FreeText(c.Text, query)) || 
                i.CustomFields.Any(f => EF.Functions.FreeText(f.Title, query)))
                .ToListAsync();
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

        public async Task DeleteAsync(int collectionId)
        {
            List<Item> items = await GetAllAsync(collectionId);
            items.ForEach(item => _context.Items.Remove(item));
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
