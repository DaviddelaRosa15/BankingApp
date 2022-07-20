using BankingApp.Core.Application.ViewModels.CreditCard;
using BankingApp.Core.Application.ViewModels.Loan;
using BankingApp.Core.Application.ViewModels.SavingAccount;
using BankingApp.Core.Application.ViewModels.Transactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingApp.Core.Application.ViewModels.Client
{
    public class ClientProductsViewModel
    {
        public List<SavingAccountViewModel> SavingAccounts { get; set; }
        public List<CreditCardViewModel> CreditCards { get; set; }
        public List<LoanViewModel> Loans { get; set; }
        public List<TransactionViewModel> Transactions { get; set; }
    }
}
