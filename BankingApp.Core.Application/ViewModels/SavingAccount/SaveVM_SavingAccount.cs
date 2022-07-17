using BankingApp.Core.Application.ViewModels.Beneficiary;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingApp.Core.Application.ViewModels.SavingAccount
{
    public class SaveVM_SavingAccount
    {
        public int SavingAccountId { get; set; }

        [Required(ErrorMessage = "You must specify an amount.")]
        [DataType(DataType.Currency)]
        public double Balance { get; set; } = 0.00;

        public string UserId { get; set; }
        public bool IsPrincipal { get; set; } = false;

        //Reference Property
        public List<BeneficiaryViewModel> Beneficiaries { get; set; }

    }
}
