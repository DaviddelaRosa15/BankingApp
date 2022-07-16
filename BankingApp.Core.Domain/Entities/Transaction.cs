using BankingApp.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingApp.Core.Domain.Entities
{
    public class Transaction : AuditableBaseEntity
    {
        public int OriginAccount { get; set; }
        public int DestinyAccount { get; set; }
        public double Amount { get; set; }
        public string TransactionType { get; set; }
    }
}
