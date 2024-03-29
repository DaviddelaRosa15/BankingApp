﻿using BankingApp.Core.Application.ViewModels.Loan;
using BankingApp.Core.Application.ViewModels.SavingAccount;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingApp.Core.Application.ViewModels.Operation
{
    public class LoanPaymentViewModel
    {
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar la cuenta a la que se le va a debitar el monto")]
        public int OriginAccount { get; set; }
        public int DestinyLoan { get; set; }
        public List<SavingAccountViewModel> AccountsOwn { get; set; }
        public double Share { get; set; }
        public bool HasError { get; set; }
        public string Error { get; set; }

    }
}
