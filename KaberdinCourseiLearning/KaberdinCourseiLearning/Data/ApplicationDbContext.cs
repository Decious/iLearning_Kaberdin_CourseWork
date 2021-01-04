using System;
using System.Collections.Generic;
using System.Text;
using KaberdinCourseiLearning.Areas.Identity;
using KaberdinCourseiLearning.Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace KaberdinCourseiLearning.Data
{
    public class ApplicationDbContext : IdentityDbContext<CustomUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Like>()
                .HasKey(o => new { o.ProductID,o.UserID});
            builder.Entity<ProductColumnValue>()
                .HasKey(o => new { o.ColumnID, o.ProductID });
            base.OnModelCreating(builder);
        }

        public DbSet<UserPage> UserPages { get; set; }
        public DbSet<ProductCollection> ProductCollections { get; set; }
        public DbSet<ProductCollectionColumn> ProductCollectionColumns { get; set; }
        public DbSet<Product> Product { get; set; }
        public DbSet<ProductColumnValue> ProductColumnValues { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Like> Likes { get; set; }
    }
}
