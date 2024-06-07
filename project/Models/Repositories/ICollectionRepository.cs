using Microsoft.AspNetCore.Identity;
using project.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace project.Models.Repositories
{
    public interface ICollectionRepository
    {
        Task<Collection> GetByIdAsync(int id);
        Task<List<Collection>> GetAllByUserAsync(IdentityUser user);
        Task<List<Collection>> GetCollectionsLargestItemAsync();
        Task<int> AddAsync(Collection collection);
        Task DeleteAsync(int id);
        Task<Collection> SortByIdAsync(int id);
        Task<Collection> SortByNameAsync(int id);
        Task<Collection> FilterByName(int id, string text);
        Task<Collection> FilterByTags(int id, string tags);
        Task SaveAsync();
        Task<Collection> EditCollection(Collection newCollection);
    }
}