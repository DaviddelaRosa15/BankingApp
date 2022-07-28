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
using System;

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
            SaveLoanViewModel loanVm = new();
            loanVm.HasError = false;
            if (vm.LoanAmount < 500 || vm.LoanAmount > 150000)
            {
                loanVm.HasError = true;
                loanVm.Error = "El monto debe estar entre: 500-15000";
                return loanVm;
            }
            if (vm.ShareQuantity != 6 && vm.ShareQuantity !=12 && vm.ShareQuantity != 18 && vm.ShareQuantity != 24)
            {
                loanVm.HasError = true;
                loanVm.Error = "Por favor: seleccione una de las cuotas disponibles";
                return loanVm;
            }
            vm.Share = vm.LoanAmount / vm.ShareQuantity;
            loanVm = await base.Add(vm);

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

        public async Task<List<SaveLoanViewModel>> GetAllLoanByIdUser(string id)
        {
            List<Loan> loan = await _loanRepository.GetAllAsync();
            List<SaveLoanViewModel> svm = _mapper.Map<List<SaveLoanViewModel>>(loan);
            return svm.Where(svm => svm.UserId == id).ToList();
        }

        public async Task<int> CountProductLoan()
        {
            List<Loan> loans = await _loanRepository.GetAllAsync();
            return loans.Count();
        }

    }
}
