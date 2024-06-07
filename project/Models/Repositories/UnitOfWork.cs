namespace project.Models.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        public ICloudRepository Cloud { get; }

        public ICollectionRepository Collections { get; }

        public ICustomFieldRepository CustomFields { get; }

        public IItemRepository Items { get; }

        public ITagRepository Tags { get; }

        public IUserRepository Users { get; }

        public UnitOfWork (
            ICloudRepository cloud, 
            ICollectionRepository collections, 
            ICustomFieldRepository customFields, 
            IItemRepository items, 
            ITagRepository tags, 
            IUserRepository users)
        {
            Cloud = cloud;
            Collections = collections;
            CustomFields = customFields;
            Items = items;
            Tags = tags;
            Users = users;
        }
    }
}
