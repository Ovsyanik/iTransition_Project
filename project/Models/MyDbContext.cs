using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using project.Models.Entities;
using System;

namespace project.Models
{
    public class MyDbContext : DbContext
    {
        public MyDbContext(DbContextOptions<MyDbContext> options) : base(options)
        {
            Database.EnsureCreatedAsync();
        }

        public DbSet<Item> Items { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<Tags> Tags { get; set; }

        public DbSet<Collection<Item>> Collections { get; set; }
    }
}
