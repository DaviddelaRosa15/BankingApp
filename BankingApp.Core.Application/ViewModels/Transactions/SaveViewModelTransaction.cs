using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingApp.Core.Application.ViewModels.Transactions
{
    public class SaveViewModelTransaction
    {
        public int Id{ get; set; }

        [Required(ErrorMessage = "You must specify an origin account, please choose one.")]
        public int OriginAccount { get; set; }

        [Required(ErrorMessage = "You must specify a destiny account, please be careful and choose one.")]
        public int DestinyAccount { get; set; }

        [Required(ErrorMessage = "You must specify a valid amount.")]
        [DataType(DataType.Currency)]
        public double Amount { get; set; }

        public string TransactionType { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    }
}
