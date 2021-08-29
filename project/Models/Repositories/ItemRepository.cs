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

        public async Task<Item> GetItemByIdAsync(int id)
        {
            return await _context.Items
                .Include(i => i.Collection).ThenInclude(c => c.User)
                .Include(i => i.Likes)
                .Include(i => i.Comments)
                .Include(i => i.Tags)
                .Include(i => i.CustomFieldValues)
                .FirstOrDefaultAsync(i => i.Id == id);
        }


        public async Task<List<Item>> GetLastItemsAsync()
        {
            return await _context.Items.Include(i => i.Tags)
                .OrderByDescending(i => i.Id).Take(5).ToListAsync();
        }


        public async Task<List<Item>> GetAllAsync(int id)
        {
            return await _context.Items
                .Where(i => i.CollectionId == id)
                .Include(i => i.CustomFieldValues)
                .Include(i => i.Tags).ToListAsync();
        }


        public List<Comment> GetCommentsAsync(int id)
        {
            return _context.Comments
                .Where(c => c.Item.Id == id)
                .OrderByDescending(c => c.Id)
                .ToList();
        }


        public async Task<List<Item>> SearchItems(string query)
        {
            return await _context.Items.Where(i => 
                EF.Functions.FreeText(i.Name, query) || 
                EF.Functions.FreeText(i.Collection.Description, query) || 
                i.Comments.Any(c => EF.Functions.FreeText(c.Text, query)) || 
                i.CustomFieldValues.Any(f => EF.Functions.FreeText(f.Value, query)))
                .ToListAsync();
        }


        public async Task<int> AddAsync(int collectionId, Item item)
        {
            Collection collection = await _context.Collections
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.Id == collectionId);
            item.Collection = collection;
            collection.Items.Add(item);
            await SaveAsync();

            return item.Id;
        }


        public async Task<Item> AddOrDeleteLikeAsync(int itemId, string userName)
        {
            Item item = await _context.Items.Include(i => i.Likes)
                .FirstOrDefaultAsync(i => i.Id == itemId);
            if (item == null) return null;
            Like like = item.Likes.FirstOrDefault(l => l.UserName == userName);
            if (like == null)
            {
                like = new Like {
                    UserName = userName,
                    Item = item,
                };
                item.Likes.Add(like);
            }
            else
            {
                item.Likes.Remove(like);
            }

            await SaveAsync();

            return item;
        }


        public async Task<Item> AddCommentAsync(int itemId, string text, string userName)
        {
            User user = await _context.Users
                .FirstOrDefaultAsync(u => u.UserName == userName);

            Item item = await _context.Items.Include(i => i.Comments)
                .FirstOrDefaultAsync(i => i.Id == itemId);

            if (item == null) return null;

            item.Comments.Add(new Comment
            {
                Author = userName,
                CreatedDate = DateTime.UtcNow,
                ItemId = itemId,
                Text = text
            });
            await SaveAsync();
            return item;
        }


        public async Task EditAsync(Item item)
        {

            await SaveAsync();
        }


        public async Task DeleteAsync(int collectionId, int id)
        {
            Collection collection = await _context.Collections.
                Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.Id == collectionId);
            collection.Items.Remove(await GetItemByIdAsync(id));
            await SaveAsync();
        }


        private async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<List<Item>> GetByTagAsync(string tag)
        {
            return await _context.Items
                .Include(i => i.CustomFieldValues)
                .Include(i => i.Tags)
                .SelectMany(t => t.Tags, (i, t) => new { Item = i, Tag = t })
                .Where(t => t.Tag.Value == tag).Select(t => t.Item).ToListAsync();
        }
    }
}
