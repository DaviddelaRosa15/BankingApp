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
    public class SavingAccountService : GenericService<SaveVM_SavingAccount,SavingAccountViewModel,SavingAccount>, ISavingAccountService
    {
        private readonly ISavingAccountRepository _accountRepository;
        private readonly IMapper _mapper;
        private readonly AuthenticationResponse _userViewModel;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SavingAccountService(ISavingAccountRepository accountRepository, IMapper mapper, IHttpContextAccessor httpContextAccessor, AuthenticationResponse userViewModel)
        :base(accountRepository, mapper)
        {
            _accountRepository = accountRepository;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            this._userViewModel = httpContextAccessor.HttpContext.Session.Get<AuthenticationResponse>("user_session");
        }

        public async Task<List<SavingAccountViewModel>> GetAllWithIncludes()
        {
            var savingAccounts  = await _accountRepository.GetAllWithIncludeAsync(new List<string>() { "Beneficiary" });
            return _mapper.Map<List<SavingAccountViewModel>>(savingAccounts);
        }
    }
}
