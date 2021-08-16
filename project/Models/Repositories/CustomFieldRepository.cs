using Microsoft.EntityFrameworkCore;
using project.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace project.Models.Repositories
{
    public class CustomFieldRepository 
    {
        private readonly MyDbContext _context;

        public CustomFieldRepository(MyDbContext context)
        {
            _context = context;
        }

        public async Task<List<CustomField>> GetAllAsync(int id)
        {
            return await _context.CustomFields.Where(cf => cf.CollectionId == id).ToListAsync();
        }

        public async Task<List<CustomFieldValue>> GetAllValuesAsync(int collectionId)
        {
            return await _context.CustomFieldValues
                .Where(cf => cf.CollectionId == collectionId)
                .ToListAsync();
        }

        public async Task AddAsync(CustomField customField)
        {
            await _context.CustomFields.AddAsync(customField);
            await SaveAsync();
        }

        public async Task<CustomFieldValue> AddCustomFieldValueAsync(CustomFieldValue value)
        {
            await _context.CustomFieldValues.AddAsync(value);
            await SaveAsync();

            return value;
        }


        public async Task DeleteValuesByIdAsync(int collectionId)
        {
            List<CustomFieldValue> values = await GetAllValuesAsync(collectionId);
            values.ForEach(value => _context.CustomFieldValues.Remove(value));
            await SaveAsync();
        }


        public async Task DeleteByIdAsync(int collectionId)
        {
            List<CustomField> fields = await GetAllAsync(collectionId);
            fields.ForEach(field => _context.CustomFields.Remove(field));
            await SaveAsync();
        }


        private async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
