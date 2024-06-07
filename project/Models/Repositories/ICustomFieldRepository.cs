using project.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace project.Models.Repositories
{
    public interface ICustomFieldRepository
    {
        Task<List<CustomField>> GetAllAsync(int id);
        Task<List<CustomFieldValue>> GetAllValuesAsync(int CollectionId);
        Task AddAsync(CustomField customField);
        Task<CustomFieldValue> AddCustomFieldValueAsync(CustomFieldValue value);
        Task DeleteValuesByIdAsync(int collectionId);
        Task DeleteByIdAsync(int collectionId);
        Task SaveAsync();
        Task EditCustomFieldAsync(int id, string title, CustomFieldType customFieldType);
        Task EditCustomFieldValueAsync(int id, string value);
    }
}