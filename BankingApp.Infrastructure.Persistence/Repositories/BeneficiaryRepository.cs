using BankingApp.Core.Domain.Entities;
using BankingApp.Infrastructure.Persistence.Contexts;
using BankingApp.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using StockApp.Core.Application.Interfaces.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Repository
{
    public class BeneficiaryRepository : GenericRepository<Beneficiary>, IBeneficiaryRepository
    {
        private readonly ApplicationContext _dbContext;

        public BeneficiaryRepository(ApplicationContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

      
    }
}
