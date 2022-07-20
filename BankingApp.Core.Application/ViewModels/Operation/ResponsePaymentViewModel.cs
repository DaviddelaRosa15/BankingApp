using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingApp.Core.Application.ViewModels.Operation
{
    public class ResponsePaymentViewModel
    {
        public string DestinyUserId { get; set; }
        public string FullNameOwner { get; set; }
        public int DestinyAccount { get; set; }
        public bool HasError { get; set; }
        public string Error { get; set; }

    }
}
