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
    public class CashAdvanceViewModel
    {
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar la tarjeta a la que se le va a debitar el monto")]
        public int OriginCard { get; set; }
        public List<CreditCardViewModel> OriginCreditCards { get; set; }
        public List<SavingAccountViewModel> AccountsOwn { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar la tarjeta a la que se le va a depositar el monto")]
        public int DestinyAccount { get; set; }

        [Required(ErrorMessage = "Debe colocar un monto válido, para procesar su pago.")]
        [RegularExpression("^[0-9]$", ErrorMessage = "Ingrese un monto válido.")]
        [Range(1, int.MaxValue, ErrorMessage = "El monto a pagar no puede ser 0")]
        [DataType(DataType.Currency)]
        public double Amount { get; set; }
        public bool HasError { get; set; }
        public string Error { get; set; }

    }
}
