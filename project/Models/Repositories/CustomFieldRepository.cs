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

        public List<CustomField> GetAll(int id)
        {
            return _context.CustomFields.Where(cf => cf.CollectionId == id).ToList();
        }

        public void Add(CustomField customField)
        {
            _context.CustomFields.Add(customField);

            Save();
        }

        private void Save()
        {
            _context.SaveChanges();
        }
    }
}
