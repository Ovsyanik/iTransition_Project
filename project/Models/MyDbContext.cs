using Microsoft.EntityFrameworkCore;
using project.Models.Entities;

namespace project.Models
{
    public class MyDbContext : DbContext
    {
        public DbSet<Tags> Tags { get; set; }
        public DbSet<Like> Likes { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Collection> Collections { get; set; }
        public DbSet<CustomField> CustomFields { get; set; }
        public DbSet<CustomFieldValue> CustomFieldValues { get; set; }

        public MyDbContext(DbContextOptions<MyDbContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Tags>()
                .HasOne(p => p.Item)
                .WithMany(t => t.Tags)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Role = 0,
                    UserName = "Roman",
                    Email = "ovsyanik_roma@mail.ru",
                    PasswordHash = "Gc8ZEFHmu0/B4H8jSh/GrNUX/Kec5PGNYmWt7BODoj6RSXkTRdeL0p+o33WghSeLzWjkjsfiIqSemRmnhvq5Tg=="
                });
        }
    }
}
