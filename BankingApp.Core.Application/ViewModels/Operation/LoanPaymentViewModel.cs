using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingApp.Core.Application.ViewModels.Operation
{
    public class LoanPaymentViewModel
    {
        public int OriginAccount { get; set; }
        public int DestinyLoan { get; set; }
        public double Share { get; set; }
        public bool HasError { get; set; }
        public string Error { get; set; }

    }
}
