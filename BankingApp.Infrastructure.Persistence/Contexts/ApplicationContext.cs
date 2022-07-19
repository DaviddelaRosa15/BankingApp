using Microsoft.EntityFrameworkCore;
using BankingApp.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BankingApp.Core.Domain.Entities;

namespace BankingApp.Infrastructure.Persistence.Contexts
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options) { }

        #region DbSets
        public DbSet<SavingAccount> SavingAccounts { get; set; }
        public DbSet<Beneficiary> Beneficiaries { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<CreditCard> CreditCards { get; set; }
        public DbSet<Loan> Loans { get; set; }
        #endregion

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            foreach (var entry in ChangeTracker.Entries<AuditableBaseEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.Id = int.Parse(DateTime.UtcNow.Ticks.ToString().Substring(9));
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

            modelBuilder.Entity<SavingAccount>()
                .ToTable("SavingAccounts");

            modelBuilder.Entity<Beneficiary>()
                .ToTable("Beneficiaries");

            modelBuilder.Entity<Loan>()
                .ToTable("Loans");

            modelBuilder.Entity<CreditCard>()
                .ToTable("CreditCards");

            modelBuilder.Entity<Transaction>()
                .ToTable("Transactions");
            #endregion

            #region Primary keys
            modelBuilder.Entity<SavingAccount>()
                .HasKey(save => save.Id);

            modelBuilder.Entity<CreditCard>()
                .HasKey(credit => credit.Id);

            modelBuilder.Entity<Loan>()
                .HasKey(loan => loan.Id);

            modelBuilder.Entity<Beneficiary>()
                .HasKey(beneficiary => beneficiary.Id);

            modelBuilder.Entity<Beneficiary>()
                .HasKey(beneficiary => beneficiary.Id);
            #endregion

            #region Relationships

            modelBuilder.Entity<SavingAccount>()
                .HasMany<Beneficiary>(save => save.beneficiaries)
                .WithOne(beneficiary => beneficiary.SavingAccount)
                .HasForeignKey(beneficiary => beneficiary.SavingAccountId)
                .OnDelete(DeleteBehavior.Cascade);

            #endregion

            #region Property configurations

            modelBuilder.Entity<SavingAccount>().
                Property(save => save.Id)
                .ValueGeneratedNever();

            modelBuilder.Entity<Transaction>().
                Property(transaction => transaction.Id)
                .ValueGeneratedNever();

            modelBuilder.Entity<Loan>().
                Property(loan => loan.Id)
                .ValueGeneratedNever();

            modelBuilder.Entity<CreditCard>().
                Property(credit => credit.Id)
                .ValueGeneratedNever();

            modelBuilder.Entity<Beneficiary>().
                Property(beneficiary => beneficiary.Id)
                .ValueGeneratedNever();
            #endregion
        }

    }
}
