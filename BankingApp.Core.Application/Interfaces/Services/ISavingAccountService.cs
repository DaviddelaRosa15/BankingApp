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
        Task<SaveVM_SavingAccount> GetPrincipalByUserId(string id);
        Task<List<SavingAccountViewModel>> GetAllViewModelWithInclude();
        Task<SaveVM_SavingAccount> GetCardByIdUserAsync(string id);
        Task<SaveVM_SavingAccount> Delete(int id);
        Task<List<SaveVM_SavingAccount>> GetAllAccountByIdUser(string id);
        Task<int> CountSavingAccout();
    }
}
