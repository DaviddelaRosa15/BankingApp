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

namespace BankingApp.Core.Application.Services
{
    public class LoanService : GenericService<SaveLoanViewModel, LoanViewModel, Loan>, ILoanService
    {
        private readonly ILoanRepository _loanRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly AuthenticationResponse _userViewModel;

        public LoanService(ILoanRepository loanRepository, IMapper mapper, IHttpContextAccessor httpContextAccessor) : base(loanRepository, mapper)
        {
            _loanRepository = loanRepository;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
            this._userViewModel = httpContextAccessor.HttpContext.Session.Get<AuthenticationResponse>("user_session");
        }

    }
}
