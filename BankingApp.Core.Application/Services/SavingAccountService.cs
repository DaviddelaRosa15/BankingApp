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
            var accountList = await _accountRepository.GetAllWithIncludeAsync(new List<string>(){ "beneficiaries" });

            return accountList.Where(x => x.UserId == userViewModel.Id).Select(account => new SavingAccountViewModel
            {
                SavingAccountId = account.Id,
                Balance = account.Balance,
                UserId = account.UserId,
                IsPrincipal = account.IsPrincipal
            }).ToList();
        }
    }
}
