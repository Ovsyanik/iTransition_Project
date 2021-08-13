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

        public async Task Add(CustomField customField)
        {
            await _context.CustomFields.AddAsync(customField);

            await Save();
        }

        private async Task Save()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<List<CustomField>> GetAll(int id)
        {
            return await _context.CustomFields.Where(cf => cf.CollectionId == id).ToListAsync();
        }
    }
}
