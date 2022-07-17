using BankingApp.Core.Application.ViewModels.Beneficiary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingApp.Core.Application.ViewModels.SavingAccount
{
    public class SavingAccountViewModel
    {
        public int SavingAccountId { get; set; }
        public double Balance { get; set; }
        public string UserId { get; set; }
        public bool IsPrincipal { get; set; }

        //Reference Property
        public List<BeneficiaryViewModel> Beneficiaries { get; set; }

    }
}
