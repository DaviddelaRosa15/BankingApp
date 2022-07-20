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
        private readonly ILoanService _loanService;

        public OperationService(ISavingAccountService savingService, ICreditCardService cardService,
            ITransactionService transactionService, IAccountService accountService, ILoanService loanService)
        {
            _savingService = savingService;
            _cardService = cardService;
            _transactionService = transactionService;
            _accountService = accountService;
            _loanService = loanService;
        }

        //Método para pago expreso y de beneficiario
        public async Task<ResponsePaymentViewModel> ExpressPay(PaymentViewModel vm)
        {
            ResponsePaymentViewModel response = new()
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

        //Método para pago de tarjeta de crédito
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
            
            if (cardDestiny.Debit == 0)
            {
                response.HasError = true;
                response.Error = $"Usted está al día con el pago de esta tarjeta";
                return response;
            }

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

        //Método para pago de prestamo
        public async Task<LoanPaymentViewModel> LoanPay(LoanPaymentViewModel vm)
        {
            LoanPaymentViewModel response = new()
            {
                HasError = false
            };

            var accountOrigin = await _savingService.GetByIdSaveViewModel(vm.OriginAccount);
            if (accountOrigin.Balance < vm.Share)
            {
                response.HasError = true;
                response.Error = $"La cuenta de origen seleccionada no tiene balance suficiente para realizar este pago";
                return response;
            }

            var loanDestiny = await _loanService.GetByIdSaveViewModel(vm.DestinyLoan);

            loanDestiny.AmountPaid += vm.Share;
            if (loanDestiny.AmountPaid == loanDestiny.LoanAmount)
            {
                loanDestiny.IsPaid = true;
            }
            accountOrigin.Balance -= vm.Share;

            await _loanService.Update(loanDestiny, loanDestiny.Id);
            await _savingService.Update(accountOrigin, accountOrigin.SavingAccountId);

            SaveViewModelTransaction transaction = new()
            {
                OriginAccount = accountOrigin.SavingAccountId,
                DestinyAccount = loanDestiny.Id,
                Amount = vm.Share,
                TransactionType = "Pay"
            };

            await _transactionService.Add(transaction);

            return response;
        }

        //Método para avance de efectivo
        public async Task<CashAdvanceViewModel> CashAdvance(CashAdvanceViewModel vm)
        {
            CashAdvanceViewModel response = new()
            {
                HasError = false
            };

            var cardOrigin = await _cardService.GetByIdSaveViewModel(vm.OriginCard);
            if (cardOrigin.AvailableCredit == 0)
            {
                response.HasError = true;
                response.Error = $"La tarjeta no tiene fondo, paguela.";
                return response;
            }
            if (cardOrigin.Limit < vm.Amount)
            {
                response.HasError = true;
                response.Error = $"El monto supera el limite de la tarjeta.";
                return response;
            }
            if (cardOrigin.AvailableCredit < vm.Amount)
            {
                response.HasError = true;
                response.Error = $"El monto supera el credito disponible de la tarjeta.";
                return response;
            }

            var accountDestiny = await _savingService.GetByIdSaveViewModel(vm.DestinyAccount);

            accountDestiny.Balance += vm.Amount;
            cardOrigin.AvailableCredit -= vm.Amount;
            cardOrigin.Debit += vm.Amount * 1.0625;

            await _savingService.Update(accountDestiny, accountDestiny.SavingAccountId);
            await _cardService.Update(cardOrigin, cardOrigin.Id);

            SaveViewModelTransaction transaction = new()
            {
                OriginAccount = cardOrigin.Id,
                DestinyAccount = accountDestiny.SavingAccountId,
                Amount = vm.Amount,
                TransactionType = "Transaction"
            };

            await _transactionService.Add(transaction);

            return response;
        }

        //Método para transferencia entre cuentas
        public async Task<AccountTransferViewModel> AccountTransfer(AccountTransferViewModel vm)
        {
            AccountTransferViewModel response = new()
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

            return response;
        }

    }
}
