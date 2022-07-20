using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingApp.Core.Application.ViewModels.Operation
{
    public class PaymentViewModel
    {
        public int OriginAccount { get; set; }
        public int DestinyAccount { get; set; }
        public double Amount { get; set; }
        public bool HasError { get; set; }
        public string Error { get; set; }
    }
}
