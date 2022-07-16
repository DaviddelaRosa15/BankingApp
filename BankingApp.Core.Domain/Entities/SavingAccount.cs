using BankingApp.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingApp.Core.Domain.Entities
{
    public class SavingAccount : AuditableBaseEntity
    {
        public double Balance { get; set; }
        public string UserId { get; set; }
        public bool IsPrincipal { get; set; }

        //Navigation Property
        public ICollection<Beneficiary> beneficiaries { get; set; }
    }
}
