using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace project.Models.Repositories
{
    public class BookRepository
    {
        private readonly MyDbContext _context;

        public BookRepository(MyDbContext context)
        {
            _context = context;
        }

        public async Task Add(Book book)
        {

            await Save();
        }

        public async Task Change(Book book)
        {

            await Save();
        }

        public async Task Delete(Book book)
        {
            await Save();
        }

        private async Task Save()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<List<Book>> GetCollections()
        {
            return new List<Book>();   
        }
    }
}
