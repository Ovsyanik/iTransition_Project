using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using project.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace project.Models.Repositories
{
    public class CollectionRepository : ICollectionRepository
    {
        private readonly ApplicationContext _context;

        public CollectionRepository(ApplicationContext context)
        {
            _context = context;
        }

        public async Task<Collection> GetByIdAsync(int id)
        {
            return await _context.Collections
                .Include(c => c.Items).ThenInclude(i => i.Tags)
                .Include(c => c.Items).ThenInclude(i => i.CustomFieldValues)
                .Include(c => c.Items).ThenInclude(i => i.Likes)
                .Include(c => c.User)
                .Include(c => c.Fields)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public Task<List<Collection>> GetAllByUserAsync(IdentityUser user)
        {
            return _context.Collections.Where(c => c.User.Id == user.Id).ToListAsync();
        }

        public async Task<List<Collection>> GetCollectionsLargestItemAsync()
        {
            return await _context.Collections
                .OrderByDescending(c => c.Items.Count)
                .Take(6)
                .ToListAsync();
        }

        public async Task<int> AddAsync(Collection collection)
        {
            await _context.Collections.AddAsync(collection);
            await SaveAsync();
            return collection.Id;
        }

        public async Task DeleteAsync(int id)
        {
            Collection collection = await GetByIdAsync(id);
            _context.Collections.Remove(collection);
            await SaveAsync();
        }

        public async Task<Collection> SortByIdAsync(int id)
        {
            Collection collection = await GetByIdAsync(id);
            if (SortViewModel.SortId)
            {
                collection.Items = collection.Items.OrderByDescending(i => i.Id).ToList();
                SortViewModel.SortId = false;
            }
            else
            {
                collection.Items = collection.Items.OrderBy(i => i.Id).ToList();
                SortViewModel.SortId = true;
            }
            return collection;
        }

        public async Task<Collection> SortByNameAsync(int id)
        {
            Collection collection = await GetByIdAsync(id);
            if (SortViewModel.SortName)
            {
                collection.Items = collection.Items.OrderByDescending(i => i.Name).ToList();
                SortViewModel.SortName = false;
            }
            else
            {
                collection.Items = collection.Items.OrderBy(i => i.Name).ToList();
                SortViewModel.SortName = true;
            }
            return collection;
        }

        public async Task<Collection> FilterByName(int id, string text)
        {
            Collection collection = await GetByIdAsync(id);
            List<Item> items = new List<Item>();
            foreach (Item item in collection.Items)
            {
                if (item.Name.Contains(text)) 
                    items.Add(item);
            }
            collection.Items = items;
            return collection;
        }

        public async Task<Collection> FilterByTags(int id, string tags)
        {
            Collection collection = await GetByIdAsync(id);
            List<Item> items = new List<Item>();
            foreach (Item item in collection.Items)
            {
                if(String.Join(", ", item.Tags).Contains(tags))
                    items.Add(item);
            }
            collection.Items = items;
            return collection;
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<Collection> EditCollection(Collection newCollection)
        {
            Collection collection = await GetByIdAsync(newCollection.Id);
            collection.Name = newCollection.Name;
            collection.Description = newCollection.Description;
            collection.Type = newCollection.Type;
            if (newCollection.PathImage != null)
            {
                collection.PathImage = newCollection.PathImage;
            }
            await SaveAsync();
            return collection;
        }
    }
}