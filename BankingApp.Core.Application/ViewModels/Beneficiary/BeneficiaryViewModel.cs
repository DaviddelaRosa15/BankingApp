using BankingApp.Core.Application.ViewModels.SavingAccount;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingApp.Core.Application.ViewModels.Beneficiary
{
    public class BeneficiaryViewModel
    {
        public int Id { get; set; }

        //Reference Properties
        public string UserId { get; set; }
        
        public int SavingAccountId { get; set; }
        public SavingAccountViewModel SavingAccount { get; set; }
    }
}
