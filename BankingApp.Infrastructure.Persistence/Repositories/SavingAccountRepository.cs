using BankingApp.Core.Domain.Entities;
using BankingApp.Infrastructure.Persistence.Contexts;
using BankingApp.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using BankingApp.Core.Application.Interfaces.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;
using BankingApp.Core.Application.ViewModels.SavingAccount;
using System.Linq;
using System;

namespace BankingApp.Infrastructure.Persistence.Repositories
{
    public class SavingAccountRepository : GenericRepository<SavingAccount>, ISavingAccountRepository
    {
        private readonly ApplicationContext _dbContext;

        public SavingAccountRepository(ApplicationContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<SaveVM_SavingAccount> GetCardByIdUserAsync(string id)
        {
            try
            {
                List<SavingAccount> creditCards = await _dbContext.Set<SavingAccount>().ToListAsync();
                return creditCards.Select(account => new SaveVM_SavingAccount
                {
                    Balance = account.Balance,
                    UserId = account.UserId,
                    IsPrincipal = account.IsPrincipal,
                    SavingAccountId = account.Id
                }).First(ac => ac.UserId == id && ac.IsPrincipal == true);
            }
            catch (Exception ex)
            {
                return null;
            }

        }

    }
}
