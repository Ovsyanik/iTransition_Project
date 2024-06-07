using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using project.Models.Entities;

namespace project.Models
{
    public class ApplicationContext : IdentityDbContext
    {
        public DbSet<Tags> Tags { get; set; }
        public DbSet<Like> Likes { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Collection> Collections { get; set; }
        public DbSet<CustomField> CustomFields { get; set; }
        public DbSet<CustomFieldValue> CustomFieldValues { get; set; }

        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Tags>()
                .HasOne(p => p.Item)
                .WithMany(t => t.Tags)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
