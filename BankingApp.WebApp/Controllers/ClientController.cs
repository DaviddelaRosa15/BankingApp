using BankingApp.Core.Application.Dtos.Account;
using BankingApp.Core.Application.Enums;
using BankingApp.Core.Application.Helpers;
using BankingApp.Core.Application.Interfaces.Services;
using BankingApp.Core.Application.ViewModels.Client;
using BankingApp.Core.Application.ViewModels.Operation;
using BankingApp.WebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace BankingApp.WebApp.Controllers
{
    [Authorize(Roles = "Client")]
    public class ClientController : Controller
    {

        private readonly IUserService _userService;
        private readonly IOperationService _operationService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly AuthenticationResponse loggedUser;
        private readonly ICreditCardService _creditCardService;
        private readonly ILoanService _loanService;
        private readonly ISavingAccountService _savingAccountService;

        public ClientController(IUserService userService, IOperationService operationService, IHttpContextAccessor httpContextAccessor,
            ICreditCardService creditCardService, ILoanService loanService, ISavingAccountService savingAccountService)
        {
            this._userService = userService;
            this._operationService = operationService;
            this.loggedUser = httpContextAccessor.HttpContext.Session.Get<AuthenticationResponse>("user_session");

            this._creditCardService = creditCardService;
            this._loanService = loanService;
            this._savingAccountService = savingAccountService;
        }

        public async Task<IActionResult> Index()
        {
            ClientProductsViewModel clientProducts = new ClientProductsViewModel();

            clientProducts.SavingAccounts = await _savingAccountService.GetAllViewModel();
            clientProducts.SavingAccounts = clientProducts.SavingAccounts.Where(saving => saving.UserId == loggedUser.Id).ToList();

            clientProducts.CreditCards = await _creditCardService.GetAllViewModel();
            clientProducts.CreditCards = clientProducts.CreditCards.Where(credit => credit.UserId == loggedUser.Id).ToList();

            clientProducts.Loans = await _loanService.GetAllViewModel();
            clientProducts.Loans = clientProducts.Loans.Where(credit => credit.UserId == loggedUser.Id).ToList();

            return View(viewName: "Home", model: clientProducts);
        }

       


    }
}
