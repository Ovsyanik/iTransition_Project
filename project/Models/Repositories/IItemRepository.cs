using Microsoft.AspNetCore.Mvc.Rendering;
using project.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace project.Models.Repositories
{
    public interface IItemRepository
    {
        Task<Item> GetByIdAsync(int id);
        Task<List<Item>> GetLastItemsAsync();
        Task<List<Item>> GetAllAsync(int id);
        Task<List<Comment>> GetCommentsAsync(int id);
        Task<List<Item>> SearchItems(string query);
        Task<int> AddAsync(int collectionId, Item item);
        Task<Item> AddOrDeleteLikeAsync(int itemId, string userName);
        Task<Item> AddCommentAsync(int itemId, string text, string userName);
        Task EditAsync(Item newItem, string[] tags, int[] fieldId, string[] values);
        Task DeleteTagAsync(int id, int idTag);
        Task DeleteAsync(int collectionId, int id);
        Task SaveAsync();
        Task<List<Item>> GetByTagAsync(string tag);
    }
}