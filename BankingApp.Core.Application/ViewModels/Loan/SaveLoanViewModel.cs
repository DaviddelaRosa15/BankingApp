using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingApp.Core.Application.ViewModels.Loan
{
    public class SaveLoanViewModel
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public double LoanAmount { get; set; }
        public double AmountPaid { get; set; }
        public double Share { get; set; }
        public bool IsPaid { get; set; }
    }
}
