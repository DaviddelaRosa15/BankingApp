using AutoMapper;
using BankingApp.Core.Application.Dtos.Account;
using BankingApp.Core.Application.Helpers;
using BankingApp.Core.Application.Interfaces.Repositories;
using BankingApp.Core.Application.Interfaces.Services;
using BankingApp.Core.Application.ViewModels.Transactions;
using BankingApp.Core.Domain.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingApp.Core.Application.Services
{
    public class TransactionService : GenericService<SaveViewModelTransaction, TransactionViewModel, Transaction>, ITransactionService
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly AuthenticationResponse _userViewModel;

        public TransactionService(ITransactionRepository transactionRepository,IMapper mapper, IHttpContextAccessor httpContextAccessor, AuthenticationResponse userViewModel)
        :base(transactionRepository, mapper)
        {
            this._transactionRepository = transactionRepository;
            this._httpContextAccessor = httpContextAccessor;
            this._mapper = mapper;
            this._userViewModel = httpContextAccessor.HttpContext.Session.Get<AuthenticationResponse>("user_session");
        }

        //public override async Task<SaveViewModelTransaction> Add(SaveViewModelTransaction vm)
        //{
        //    vm.        //We should include a reference of who does this transaction. 
        //    return await base.Add(vm);
        //}

    }
}
