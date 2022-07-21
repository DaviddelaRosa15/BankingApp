using BankingApp.Core.Application.ViewModels.SavingAccount;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingApp.Core.Application.ViewModels.Beneficiary
{
    public class SaveViewModelBeneficiary
    {
        public int Id { get; set; }

        //Reference Properties
        public string UserId { get; set; }
        
        [Required(ErrorMessage = "You must speciafy an account.")]
        [RegularExpression("^[0-9]{9}$", ErrorMessage = "Please Insert a valid account.")]
        public int SavingAccountId { get; set; }
    }
}
