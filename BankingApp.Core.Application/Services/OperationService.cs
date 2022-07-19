using BankingApp.Core.Application.Dtos.Account;
using BankingApp.Core.Application.Interfaces.Services;
using BankingApp.Core.Application.ViewModels.Operation;
using BankingApp.Core.Application.ViewModels.SavingAccount;
using BankingApp.Core.Application.ViewModels.Transactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingApp.Core.Application.Services
{
    public class OperationService : IOperationService
    {

        private readonly ISavingAccountService _savingService;
        private readonly ICreditCardService _cardService;
        private readonly ITransactionService _transactionService;
        private readonly IAccountService _accountService;

        public OperationService(ISavingAccountService savingService, ICreditCardService cardService, ITransactionService transactionService, IAccountService accountService)
        {
            _savingService = savingService;
            _cardService = cardService;
            _transactionService = transactionService;
            _accountService = accountService;
        }

        public async Task<ResponseExpressPaymentViewModel> ExpressPay(ExpressPaymentViewModel vm)
        {
            ResponseExpressPaymentViewModel response = new()
            {
                HasError = false
            };

            if (vm.OriginAccount == vm.DestinyAccount)
            {
                response.HasError = true;
                response.Error = $"La cuentas de destino y de origen ingresada son las mismas";
                return response;
            }

            var accountDestiny = await _savingService.GetByIdSaveViewModel(vm.DestinyAccount);
            if (accountDestiny == null)
            {
                response.HasError = true;
                response.Error = $"La cuenta de destino ingresada no existe.";
                return response;
            }

            var accountOrigin = await _savingService.GetByIdSaveViewModel(vm.OriginAccount);
            if (accountOrigin.Balance < vm.Amount)
            {
                response.HasError = true;
                response.Error = $"La cuenta de origen seleccionada no tiene balance suficiente para realizar este pago";
                return response;
            }

            accountOrigin.Balance -= vm.Amount;
            await _savingService.Update(accountOrigin, accountOrigin.SavingAccountId);

            accountDestiny.Balance += vm.Amount;
            await _savingService.Update(accountDestiny, accountDestiny.SavingAccountId);

            SaveViewModelTransaction transaction = new()
            {
                OriginAccount = accountOrigin.SavingAccountId,
                DestinyAccount = accountDestiny.SavingAccountId,
                Amount = vm.Amount,
                TransactionType = "Transaction"
            };

            await _transactionService.Add(transaction);

            AuthenticationResponse user = await _accountService.GetUserById(accountDestiny.UserId);
            response.FullNameOwner = user.FirstName + " " + user.LastName;
            response.DestinyAccount = accountDestiny.SavingAccountId;

            return response;
        }

        public async Task<CreditPaymentViewModel> CreditCardPay(CreditPaymentViewModel vm)
        {
            CreditPaymentViewModel response = new()
            {
                HasError = false
            };

            var accountOrigin = await _savingService.GetByIdSaveViewModel(vm.OriginAccount);
            if (accountOrigin.Balance < vm.Amount)
            {
                response.HasError = true;
                response.Error = $"La cuenta de origen seleccionada no tiene balance suficiente para realizar este pago";
                return response;
            }

            var cardDestiny = await _cardService.GetByIdSaveViewModel(vm.DestinyCard);

            cardDestiny.Debit -= vm.Amount;
            accountOrigin.Balance -= vm.Amount;

            if (cardDestiny.Debit == 0)
            {
                cardDestiny.AvailableCredit = cardDestiny.Limit;
            }
            if (cardDestiny.Debit < 0)
            {
                accountOrigin.Balance += cardDestiny.Debit * -1;
                cardDestiny.Debit = 0;
                cardDestiny.AvailableCredit = cardDestiny.Limit;
            }

            await _cardService.Update(cardDestiny, cardDestiny.Id);
            await _savingService.Update(accountOrigin, accountOrigin.SavingAccountId);

            SaveViewModelTransaction transaction = new()
            {
                OriginAccount = accountOrigin.SavingAccountId,
                DestinyAccount = cardDestiny.Id,
                Amount = vm.Amount,
                TransactionType = "Pay"
            };

            await _transactionService.Add(transaction);

            return response;
        }
    }
}
