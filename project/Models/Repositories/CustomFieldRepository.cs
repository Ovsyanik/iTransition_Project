using Microsoft.EntityFrameworkCore;
using project.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace project.Models.Repositories
{
    public class CustomFieldRepository : ICustomFieldRepository
    {
        private readonly ApplicationContext _db;

        public CustomFieldRepository(ApplicationContext db)
        {
            _db = db;
        }

        public async Task<List<CustomField>> GetAllAsync(int id)
        {
            return await _db.CustomFields.Where(c => c.CollectionId == id).ToListAsync();
        }

        public async Task<List<CustomFieldValue>> GetAllValuesAsync(int collectionId)
        {
            return await _db.CustomFieldValues
                .Where(c => c.CollectionId == collectionId)
                .ToListAsync();
        }

        public async Task AddAsync(CustomField customField)
        {
            await _db.CustomFields.AddAsync(customField);
            await SaveAsync();
        }

        public async Task<CustomFieldValue> AddCustomFieldValueAsync(CustomFieldValue value)
        {
            await _db.CustomFieldValues.AddAsync(value);
            await SaveAsync();

            return value;
        }

        public async Task DeleteValuesByIdAsync(int collectionId)
        {
            List<CustomFieldValue> values = await GetAllValuesAsync(collectionId);
            values.ForEach(v => _db.CustomFieldValues.Remove(v));
            await SaveAsync();
        }


        public async Task DeleteByIdAsync(int collectionId)
        {
            List<CustomField> fields = await GetAllAsync(collectionId);
            fields.ForEach(f => _db.CustomFields.Remove(f));
            await SaveAsync();
        }


        public async Task SaveAsync()
        {
            await _db.SaveChangesAsync();
        }

        public async Task EditCustomFieldAsync(int id, string title, CustomFieldType customFieldType)
        {
            CustomField customField = await _db.CustomFields
                .FirstOrDefaultAsync(cf => cf.Id == id);
            customField.Title = title;
            customField.CustomFieldType = customFieldType;
            await SaveAsync();
        }

        public async Task EditCustomFieldValueAsync(int id, string value)
        {
            CustomFieldValue customField = await _db.CustomFieldValues
                .FirstOrDefaultAsync(c => c.Id == id);
            customField.Value = value;
            await SaveAsync();
        }
    }
}
