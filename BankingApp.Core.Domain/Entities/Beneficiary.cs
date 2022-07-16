using BankingApp.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingApp.Core.Domain.Entities
{
    public class Beneficiary : AuditableBaseEntity
    {
        public string UserId { get; set; }

        //Navigation Property
        public int SavingAccountId { get; set; }
        public SavingAccount savingAccount { get; set; }
    }
}
