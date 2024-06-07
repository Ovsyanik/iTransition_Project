using Microsoft.Extensions.DependencyInjection;
using project.Models.Repositories;

namespace project
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IItemRepository, ItemRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddTransient<ICloudRepository, CloudRepository>();
            services.AddScoped<ITagRepository, TagRepository>();
            services.AddScoped<ICollectionRepository, CollectionRepository>();
            services.AddTransient<ICustomFieldRepository, CustomFieldRepository>();
            services.AddTransient<IUnitOfWork, UnitOfWork>();

            return services;
        }
    }
}
