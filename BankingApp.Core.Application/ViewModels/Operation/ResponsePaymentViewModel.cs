using BankingApp.Core.Application.ViewModels.SavingAccount;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingApp.Core.Application.ViewModels.Operation
{
    public class ResponsePaymentViewModel
    {
        public string FullNameOwner { get; set; }
        public SaveVM_SavingAccount OriginAccount { get; set; }
        public SaveVM_SavingAccount DestinyAccount { get; set; }
        public double Amount { get; set; }
        public bool HasError { get; set; }
        public string Error { get; set; }

    }
}
