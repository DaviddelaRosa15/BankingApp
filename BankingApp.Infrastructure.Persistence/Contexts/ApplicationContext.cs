using Microsoft.EntityFrameworkCore;
using BankingApp.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BankingApp.Infrastructure.Persistence.Contexts
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options) { }

        //public DbSet<Product> Products { get; set; }
        //public DbSet<Category> Categories { get; set; }    

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            foreach (var entry in ChangeTracker.Entries<AuditableBaseEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.Created = DateTime.Now;
                        entry.Entity.CreatedBy = "DefaultAppUser";
                        break;
                    case EntityState.Modified:
                        entry.Entity.LastModified = DateTime.Now;
                        entry.Entity.LastModifiedBy = "DefaultAppUser";
                        break;
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //FLUENT API

            #region Tables

            //modelBuilder.Entity<Product>()
            //    .ToTable("Products");

            //modelBuilder.Entity<Category>()
            //    .ToTable("Categories");    
            #endregion

            #region Primary keys
            //modelBuilder.Entity<Product>()
            //    .HasKey(product => product.Id);

            //modelBuilder.Entity<Category>()
            //    .HasKey(category => category.Id);      
            #endregion

            #region Relationships
            //modelBuilder.Entity<Category>()
            //    .HasMany<Product>(category => category.Products)
            //    .WithOne(product => product.Category)
            //    .HasForeignKey(product => product.CategoryId)
            //    .OnDelete(DeleteBehavior.Cascade);
      
            #endregion

            #region Property configurations

            #region Products

            //modelBuilder.Entity<Product>().
            //    Property(product => product.Name)
            //    .IsRequired();

            //modelBuilder.Entity<Product>().
            //   Property(product => product.Price)
            //   .IsRequired();

            #endregion

            #region categories
            //modelBuilder.Entity<Category>().
            //  Property(category => category.Name)
            //  .IsRequired()
            //  .HasMaxLength(100);
            #endregion           

            #endregion
        }

    }
}
