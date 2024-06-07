namespace project.Models.Repositories
{
    public interface IUnitOfWork
    {
        ICloudRepository Cloud { get; }
        ICollectionRepository Collections { get; }
        ICustomFieldRepository CustomFields { get; }
        IItemRepository Items { get; }
        ITagRepository Tags { get; }
        IUserRepository Users { get; }
    }
}
