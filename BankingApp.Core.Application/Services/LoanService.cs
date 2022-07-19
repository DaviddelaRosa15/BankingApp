using AutoMapper;
using Microsoft.AspNetCore.Http;
using BankingApp.Core.Application.Dtos.Account;
using BankingApp.Core.Application.Helpers;
using BankingApp.Core.Application.Interfaces.Repositories;
using BankingApp.Core.Application.Interfaces.Services;
using BankingApp.Core.Application.ViewModels.User;
using BankingApp.Core.Domain.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BankingApp.Core.Application.ViewModels.Loan;
using BankingApp.Core.Application.ViewModels.SavingAccount;

namespace BankingApp.Core.Application.Services
{
    public class LoanService : GenericService<SaveLoanViewModel, LoanViewModel, Loan>, ILoanService
    {
        private readonly ILoanRepository _loanRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly AuthenticationResponse _userViewModel;
        private readonly ISavingAccountService _savingService;

        public LoanService(ILoanRepository loanRepository, IMapper mapper,
            IHttpContextAccessor httpContextAccessor, ISavingAccountService savingService) : base(loanRepository, mapper)
        {
            _loanRepository = loanRepository;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
            this._userViewModel = httpContextAccessor.HttpContext.Session.Get<AuthenticationResponse>("user_session");
            _savingService = savingService;
        }

        public override async Task<SaveLoanViewModel> Add(SaveLoanViewModel vm)
        {
            vm.Share = vm.LoanAmount / vm.ShareQuantity;
            SaveLoanViewModel loanVm = await base.Add(vm);

            SaveVM_SavingAccount save = await _savingService.GetPrincipalByUserId(vm.UserId);
            save.Balance += vm.LoanAmount;
            await _savingService.Update(save, save.SavingAccountId);

            return loanVm;
        }

    }
}
