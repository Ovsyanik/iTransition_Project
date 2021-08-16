﻿using Microsoft.EntityFrameworkCore;
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

        public async Task<Collection> GetByIdAsync(int id)
        {
            return await _context.Collections.Include(col => col.Fields)
                .FirstOrDefaultAsync(col => col.Id == id);
        }

        public List<Collection> GetAllByUser(User user)
        {
            return _context.Collections
                .Where(col => col.User.Id == user.Id).ToList();
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


        private async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}