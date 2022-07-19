using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingApp.Core.Application.ViewModels.Operation
{
    public class CreditPaymentViewModel
    {
        public int OriginAccount { get; set; }
        public int DestinyCard { get; set; }
        public double Amount { get; set; }
        public bool HasError { get; set; }
        public string Error { get; set; }

    }
}
