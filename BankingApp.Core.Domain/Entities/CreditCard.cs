using BankingApp.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingApp.Core.Domain.Entities
{
    public class CreditCard : AuditableBaseEntity
    {
        public string UserId { get; set; }
        public double AvailableCredit { get; set; }
        public double Debit { get; set; }
        public double Limit { get; set; }
    }
}
