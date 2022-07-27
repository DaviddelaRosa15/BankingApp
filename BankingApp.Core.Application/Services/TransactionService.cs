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
        private readonly IMapper _mapper;

        public TransactionService(ITransactionRepository transactionRepository, IMapper mapper)
        : base(transactionRepository, mapper)
        {
            _transactionRepository = transactionRepository;
            _mapper = mapper;
        }

        //public override async Task<SaveViewModelTransaction> Add(SaveViewModelTransaction vm)
        //{
        //    vm.        //We should include a reference of who does this transaction. 
        //    return await base.Add(vm);
        //}
        public async Task<CountTransaction> CountTransaction()
        {
            CountTransaction countTransaction = new();
            List<Transaction> transactions = await _transactionRepository.GetAllAsync();
            countTransaction.TransationTotal = transactions.Count;
            foreach (Transaction transaction in transactions)
            {
                if (transaction.Created.ToString("dd-MM-yy").Equals(DateTime.Now.ToString("dd-MM-yy")))
                {
                    countTransaction.TransationDaily += 1;
                }
            }

            return countTransaction;

        }
    }
}
