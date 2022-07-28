using AutoMapper;
using BankingApp.Core.Application.Dtos.Account;
using BankingApp.Core.Application.Helpers;
using BankingApp.Core.Application.Interfaces.Repositories;
using BankingApp.Core.Application.Interfaces.Services;
using BankingApp.Core.Application.ViewModels.SavingAccount;
using BankingApp.Core.Domain.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingApp.Core.Application.Services
{
    public class SavingAccountService : GenericService<SaveVM_SavingAccount, SavingAccountViewModel, SavingAccount>, ISavingAccountService
    {
        private readonly ISavingAccountRepository _accountRepository;
        private readonly IMapper _mapper;
        private readonly AuthenticationResponse userViewModel;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SavingAccountService(ISavingAccountRepository accountRepository, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        : base(accountRepository, mapper)
        {
            _accountRepository = accountRepository;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            userViewModel = _httpContextAccessor.HttpContext.Session.Get<AuthenticationResponse>("user_session");
        }

        public override async Task Delete(int id)
        {
            var account = await GetByIdSaveViewModel(id);
            var principal = await GetPrincipalByUserId(account.UserId);

            principal.Balance += account.Balance;
            await Update(principal, principal.SavingAccountId);

            await base.Delete(id);
        }

        public async Task<SaveVM_SavingAccount> GetPrincipalByUserId(string id)
        {
            var accountList = await _accountRepository.GetAllAsync();

            SaveVM_SavingAccount save = new();

            foreach (var item in accountList.Where(a => a.UserId == id && a.IsPrincipal == true))
            {
                save.SavingAccountId = item.Id;
                save.Balance = item.Balance;
                save.UserId = item.UserId;
                save.IsPrincipal = item.IsPrincipal;
            }

            return save;
        }
        public async Task<List<SavingAccountViewModel>> GetAllViewModelWithInclude()
        {
            var accountList = await _accountRepository.GetAllWithIncludeAsync(new List<string>() { "beneficiaries" });

            return accountList.Where(x => x.UserId == userViewModel.Id).OrderBy(n => n.Created).Select(account => new SavingAccountViewModel
            {
                SavingAccountId = account.Id,
                Balance = account.Balance,
                UserId = account.UserId,
                IsPrincipal = account.IsPrincipal
            }).ToList();
        }
        public async Task<List<SaveVM_SavingAccount>> GetAllAccountByIdUser(string id)
        {
            List<SavingAccount> credit = await _accountRepository.GetAllAsync();
            List<SaveVM_SavingAccount> svm = _mapper.Map<List<SaveVM_SavingAccount>>(credit);

            return svm.Where(svm => svm.UserId == id).OrderBy(n => !n.IsPrincipal).ToList();
        }
        public async Task<int> CountSavingAccout()
        {
            List<SavingAccount> account = await _accountRepository.GetAllAsync();
            return account.Count();
        }
    }
}
