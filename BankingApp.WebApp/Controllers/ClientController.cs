using BankingApp.Core.Application.Dtos.Account;
using BankingApp.Core.Application.Enums;
using BankingApp.Core.Application.Helpers;
using BankingApp.Core.Application.Interfaces.Services;
using BankingApp.Core.Application.ViewModels.Beneficiary;
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
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BankingApp.WebApp.Controllers
{
    [Authorize(Roles = "Client")]
    public class ClientController : Controller
    {

        private readonly IUserService _userService;
        private readonly IOperationService _operationService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly AuthenticationResponse _loggedUser;
        private readonly ICreditCardService _creditCardService;
        private readonly ILoanService _loanService;
        private readonly ISavingAccountService _savingAccountService;
        private readonly IBeneficiaryService _beneficiaryService;
        
        public ClientController(IUserService userService, IOperationService operationService, IHttpContextAccessor httpContextAccessor,
            ICreditCardService creditCardService, ILoanService loanService, ISavingAccountService savingAccountService, IBeneficiaryService beneficiaryService)
        {
            this._userService = userService;
            this._operationService = operationService;
            this._loggedUser = httpContextAccessor.HttpContext.Session.Get<AuthenticationResponse>("user_session");

            this._creditCardService = creditCardService;
            this._loanService = loanService;
            this._savingAccountService = savingAccountService;
            this._beneficiaryService = beneficiaryService;

        }

        public async Task<IActionResult> Index()
        {
            ClientProductsViewModel clientProducts = new ClientProductsViewModel();

            clientProducts.SavingAccounts = await _savingAccountService.GetAllViewModel();
            clientProducts.SavingAccounts = clientProducts.SavingAccounts.Where(saving => saving.UserId == _loggedUser.Id).ToList();

            clientProducts.CreditCards = await _creditCardService.GetAllViewModel();
            clientProducts.CreditCards = clientProducts.CreditCards.Where(credit => credit.UserId == _loggedUser.Id).ToList();

            clientProducts.Loans = await _loanService.GetAllViewModel();
            clientProducts.Loans = clientProducts.Loans.Where(credit => credit.UserId == _loggedUser.Id).ToList();

            return View(viewName: "Home", model: clientProducts);
        }

        
        public async Task<IActionResult> Beneficiary()
        {
            List<BeneficiaryViewModel> beneficiaries = await _beneficiaryService.GetAllViewModelWithInclude();

            beneficiaries = beneficiaries.Select(ben => new  BeneficiaryViewModel() { 
                BeneficiaryName =  _userService.GetUserById(ben.SavingAccount.UserId).Result.FirstName,
                BeneficiaryLastName = _userService.GetUserById(ben.SavingAccount.UserId).Result.LastName,
                Id = ben.Id,
                SavingAccountId = ben.SavingAccountId
            }).ToList();

            return View(viewName: "Beneficiary", model: beneficiaries);
        }


        [HttpPost]
        public async Task<IActionResult> Beneficiary(string SavingAccountId)
        {
            bool isValidAccount = new Regex("^[0-9]{9}$", RegexOptions.None).IsMatch(SavingAccountId);

            if (!isValidAccount)
            {

                ViewBag.beneficiaryStatus = new BeneficiaryViewModel()
                {
                    HasError = true,
                    Error = "Please enter a valid account identifier."
                };

                return RedirectToAction(controllerName: "Client", actionName: "Beneficiary");

                //return View(viewName: "Beneficiary", model: new BeneficiaryViewModel() {
                //    HasError = true,
                //    Error = "Please enter a valid account identifier."
                //});
            }

            var beneficiary = await _beneficiaryService.GetByIdSaveViewModel(int.Parse(SavingAccountId));

            if(beneficiary == null)
            {
                ViewBag.beneficiaryStatus = new BeneficiaryViewModel()
                {
                    HasError = true,
                    Error = $"This account {SavingAccountId} does not exist..."
                };

                return RedirectToAction(controllerName: "Client", actionName: "Beneficiary");

                //return View(viewName: "Beneficiary", model: new BeneficiaryViewModel()
                //{
                //    HasError = true,
                //    Error = $"This account {SavingAccountId} does not exist..."
                //});
            }

            await _beneficiaryService.Add(new SaveViewModelBeneficiary() {
                SavingAccountId = beneficiary.Id,
                UserId = _loggedUser.Id
            });

            return RedirectToAction(controllerName: "Client", actionName: "Beneficiary");
        }


        public async Task<IActionResult> ExpressPay()
        {
            PaymentViewModel model = new();
            model.AccountsOwn = await _savingAccountService.GetAllViewModelWithInclude();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ExpressPay(PaymentViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                vm.AccountsOwn = await _savingAccountService.GetAllViewModelWithInclude();
                return View(vm);
            }

            ResponsePaymentViewModel model = await _operationService.PayValidation(vm);


            if (model.HasError)
            {
                vm.HasError = true;
                vm.Error = model.Error;
                vm.AccountsOwn = await _savingAccountService.GetAllViewModelWithInclude();
                return View(vm);
            }

            return View("ConfirmExpressPay", model);
        }

        public async Task<IActionResult> MakeExpressPay(ResponsePaymentViewModel vm)
        {
            await _operationService.Pay(vm);
            return RedirectToRoute(new { controller = "Client", action = "Index"});
        }
    }
}
