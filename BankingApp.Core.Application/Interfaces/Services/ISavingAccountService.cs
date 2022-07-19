using BankingApp.Core.Application.ViewModels.SavingAccount;
using BankingApp.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingApp.Core.Application.Interfaces.Services
{
    public interface ISavingAccountService : IGenericService<SaveVM_SavingAccount,SavingAccountViewModel,SavingAccount>
    {
    }
}
