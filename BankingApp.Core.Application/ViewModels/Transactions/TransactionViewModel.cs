using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingApp.Core.Application.ViewModels.Transactions
{
    public class TransactionViewModel
    {
        public int Id{ get; set; }
        public int OriginAccount { get; set; }
        public int DestinyAccount { get; set; }
        public double Amount { get; set; }
        public string TransactionType { get; set; }
        public DateTime CreatedAt { get; set; }


    }
}
