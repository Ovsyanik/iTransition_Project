using project.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace project.Models.Repositories
{
    public interface ITagRepository
    {
        Task<List<Tags>> GetAllAsync();
        Task<List<string>> GetAllValuesAsync();
        Task<Tags> AddAsync(Tags tag);
        Task SaveAsync();
    }
}