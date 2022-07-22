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
        private readonly IMapper _mapper;
        private readonly ISavingAccountService _savingService;
        private readonly AuthenticationResponse userViewModel;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LoanService(ILoanRepository loanRepository, IMapper mapper,
            ISavingAccountService savingService, IHttpContextAccessor httpContextAccessor) : base(loanRepository, mapper)
        {
            _loanRepository = loanRepository;
            _mapper = mapper;
            _savingService = savingService;
            _httpContextAccessor = httpContextAccessor;
            userViewModel = _httpContextAccessor.HttpContext.Session.Get<AuthenticationResponse>("user_session");
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

        public async Task<List<LoanViewModel>> GetAllViewModelWithInclude()
        {

            var result = await _loanRepository.GetAllWithIncludeAsync(new List<string>() {});

            return result.Where(x => x.UserId == userViewModel.Id).Select(loan => new LoanViewModel()
            {
                Id = loan.Id,
                LoanAmount = loan.LoanAmount,
                AmountPaid = loan.AmountPaid,
                Share = loan.Share,
                IsPaid = loan.IsPaid
                
            }).ToList();

        }
    }
}
