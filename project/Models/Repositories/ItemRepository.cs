﻿using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using project.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace project.Models.Repositories
{
    public class ItemRepository : IItemRepository
    {
        private readonly ApplicationContext _db;

        public ItemRepository(ApplicationContext db)
        {
            _db = db;
        }

        public async Task<Item> GetByIdAsync(int id)
        {
            return await _db.Items
                .Include(i => i.Collection).ThenInclude(c => c.User)
                .Include(i => i.Likes)
                .Include(i => i.Comments)
                .Include(i => i.Tags)
                .Include(i => i.CustomFieldValues)
                .FirstOrDefaultAsync(i => i.Id == id);
        }
        public async Task<List<Item>> GetLastItemsAsync()
        {
            return await _db.Items
                .Include(i => i.Tags)
                .OrderByDescending(i => i.Id)
                .Take(5)
                .ToListAsync();
        }

        public async Task<List<Item>> GetAllAsync(int id)
        {
            return await _db.Items
                .Where(i => i.CollectionId == id)
                .Include(i => i.CustomFieldValues)
                .Include(i => i.Tags)
                .ToListAsync();
        }

        public async Task<List<Comment>> GetCommentsAsync(int id)
        {
            return await _db.Comments
                .Where(c => c.Item.Id == id)
                .OrderByDescending(c => c.Id)
                .ToListAsync();
        }

        public async Task<List<Item>> SearchItems(string query)
        {
            return await _db.Items.Where(i => 
                EF.Functions.FreeText(i.Name, query) || 
                EF.Functions.FreeText(i.Collection.Description, query) || 
                i.Comments.Any(c => EF.Functions.FreeText(c.Text, query)) || 
                i.CustomFieldValues.Any(f => EF.Functions.FreeText(f.Value, query)))
                .ToListAsync();
        }

        public async Task<int> AddAsync(int collectionId, Item item)
        {
            Collection collection = await _db.Collections
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.Id == collectionId);
            item.Collection = collection;
            collection.Items.Add(item);
            await SaveAsync();

            return item.Id;
        }

        public async Task<Item> AddOrDeleteLikeAsync(int itemId, string userName)
        {
            Item item = await _db.Items.Include(i => i.Likes)
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
            IdentityUser user = await _db.Users
                .FirstOrDefaultAsync(u => u.UserName == userName);

            Item item = await _db.Items.Include(i => i.Comments)
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

        public async Task EditAsync(Item newItem, string[] tags, int[] fieldId, string[] values)
        {
            Item item = await GetByIdAsync(newItem.Id);
            item.Name = newItem.Name;
            for(int i = 0; i < fieldId.Length; i++)
            {
                var customFieldValues = item.CustomFieldValues
                    .Where(c => c.Id == fieldId[i])
                    .FirstOrDefault();
                customFieldValues.Value = values[i];
            }
            for (int i = 0; i < tags.Length; i++)
            {
                if (item.Tags.Where(t => t.Value == tags[i]).FirstOrDefault() == null)
                {
                    item.Tags.Add(new Tags { Item = item, Value = tags[i] });
                }
            }
        
            await SaveAsync();
        }

        public async Task DeleteTagAsync(int id, int idTag)
        {
            Item item = await GetByIdAsync(id);
            item.Tags.Remove(item.Tags.Where(t => t.Id == idTag).FirstOrDefault());
            await SaveAsync();
        }

        public async Task DeleteAsync(int collectionId, int id)
        {
            Collection collection = await _db.Collections.
                Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.Id == collectionId);
            collection.Items.Remove(await GetByIdAsync(id));
            await SaveAsync();
        }

        public async Task SaveAsync()
        {
            await _db.SaveChangesAsync();
        }

        public async Task<List<Item>> GetByTagAsync(string tag)
        {
            return await _db.Items
                .Include(i => i.CustomFieldValues)
                .Include(i => i.Tags)
                .SelectMany(t => t.Tags, (i, t) => new { Item = i, Tag = t })
                .Where(t => t.Tag.Value == tag)
                .Select(t => t.Item)
                .ToListAsync();
        }
    }
}