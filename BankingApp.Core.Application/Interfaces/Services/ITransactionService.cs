using BankingApp.Core.Application.ViewModels.Transactions;
using BankingApp.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingApp.Core.Application.Interfaces.Services
{
    public interface ITransactionService : IGenericService<SaveViewModelTransaction, TransactionViewModel, Transaction>
    {
    }
}
