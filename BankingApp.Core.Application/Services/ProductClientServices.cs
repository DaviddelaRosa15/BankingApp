using BankingApp.Core.Application.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingApp.Core.Application.Services
{
    public class ProductClientServices : IProductClient
    {
        private readonly ICreditCardService _creditCardService;
        private readonly ILoanService _loanService;
        private readonly ISavingAccountService _accountService;
        public ProductClientServices(ICreditCardService creditCardService,
            ISavingAccountService accountService, ILoanService loanSercice
            )
        {
            _accountService = accountService;
            _creditCardService = creditCardService;
            _loanService = loanSercice;
        }
        public async Task<int> CountProductAsigned()
        {
            int CountProduct = await _creditCardService.CountCreditCard() +
            await _accountService.CountSavingAccout() +
            await _loanService.CountProductLoan();
            return CountProduct;
        }



    }
}
