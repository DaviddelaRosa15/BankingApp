using BankingApp.Core.Application.ViewModels.CreditCard;
using BankingApp.Core.Application.ViewModels.SavingAccount;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingApp.Core.Application.ViewModels.Operation
{
    public class CreditPaymentViewModel
    {
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar la cuenta a la que se le va a debitar el monto")]
        public int OriginAccount { get; set; }
        public int DestinyCard { get; set; }
        public List<SavingAccountViewModel> AccountsOwn { get; set; }

        [Required(ErrorMessage = "Debe colocar el monto a pagar")]
        [DataType(DataType.Currency)]
        public double Amount { get; set; }
        public bool HasError { get; set; }
        public string Error { get; set; }

    }
}
