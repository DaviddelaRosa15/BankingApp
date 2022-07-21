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

            return View(viewName: "Index", model: clientProducts);
        }

        #region Mantenimiento de beneficiarios
        //Estos 4 primeros son del mantenimiento de usuario.
        public async Task<IActionResult> MyBeneficiaries()
        {
            List<BeneficiaryViewModel> beneficiaries = await _beneficiaryService.GetAllViewModelWithInclude();

            beneficiaries = beneficiaries.Select(ben => new  BeneficiaryViewModel() { 
                BeneficiaryName =  _userService.GetUserById(ben.SavingAccount.UserId).Result.FirstName,
                BeneficiaryLastName = _userService.GetUserById(ben.SavingAccount.UserId).Result.LastName,
                Id = ben.Id,
                SavingAccountId = ben.SavingAccountId
            }).ToList();

            return View(viewName: "MyBeneficiary", model: beneficiaries);
        }

        [HttpPost]
        public async Task<IActionResult> MyBeneficiaries(string SavingAccountId)
        {
            
            if (!ValidationHelper.IsValidProductID(SavingAccountId))
            {

                ViewBag.beneficiaryStatus = new BeneficiaryViewModel()
                {
                    HasError = true,
                    Error = "Please enter a valid account identifier."
                }; //En caso de que esto no funcione hacer el inverso, enviar el list por un viewBag.

                return RedirectToAction(controllerName: "Client", actionName: "MyBeneficiary");

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

        [HttpGet]
        public async Task<IActionResult> BeneficiaryDelete(string id)
        {
            List<BeneficiaryViewModel> beneficiaries = await _beneficiaryService.GetAllViewModelWithInclude();
            BeneficiaryViewModel beneficiary;

            if (!ValidationHelper.IsValidProductID(id))
            {
                return View(viewName: "ConfirmBeneficiaryDelete", model: new BeneficiaryViewModel()
                {
                    HasError = true,
                    Error = "Ha habido un error a la hora de confirmar la eliminación, si esto persiste contacte inmediatamente al administrado."
                });
            }

            int identifier = int.Parse(id);

            beneficiary = beneficiaries.FirstOrDefault(benf => benf.Id == identifier && benf.UserId == _loggedUser.Id);

            beneficiary.BeneficiaryName = _userService.GetUserById(beneficiary.SavingAccount.UserId)
                                          .Result.FirstName;

            beneficiary.BeneficiaryLastName = _userService.GetUserById(beneficiary.SavingAccount.UserId)
                                              .Result.LastName;

            return View(viewName: "ConfirmBeneficiaryDelete", model: beneficiary);

        }

        [HttpPost]
        public async Task<IActionResult> BeneficiaryDelete(BeneficiaryViewModel beneficiary)
        {
            await _beneficiaryService.Delete(beneficiary.Id);
            return RedirectToAction(controllerName: "Client", actionName: "MyBeneficiary");
        }

        #endregion


        #region Pago de beneficiarios y expresos
        
        [HttpGet]
        //Muestra los beneficiarios a los que se les quiere pagar.
        public async Task<IActionResult> MyBeneficiariesPay()
        {
            List<BeneficiaryViewModel> beneficiaries = await _beneficiaryService.GetAllViewModelWithInclude();

            beneficiaries = beneficiaries.Select(ben => new BeneficiaryViewModel()
            {
                BeneficiaryName = _userService.GetUserById(ben.SavingAccount.UserId).Result.FirstName,
                BeneficiaryLastName = _userService.GetUserById(ben.SavingAccount.UserId).Result.LastName,
                Id = ben.Id,
                SavingAccountId = ben.SavingAccountId
            }).ToList();

            return View(viewName: "MyBeneficiariesPay", model: beneficiaries);
        }


        [HttpGet]
        public async Task<IActionResult> BeneficiaryPay(string SavingAccountId)
        {
            //Pago para el benecifiario.
             PaymentViewModel model = new();
              
            if (ValidationHelper.IsValidProductID(SavingAccountId))
            {
                model.DestinyAccount = int.Parse(SavingAccountId);
                model.AccountsOwn = await _savingAccountService.GetAllViewModelWithInclude();
                return View(model);
            }
            else
            {
                model.HasError = true;
                model.Error = "Ha habido un error a la hora de confirmar la su pago, si esto persiste contacte inmediatamente al administrado.";
                return View(viewName: "MyBeneficiariesPay", model);
            }

        }

        [HttpPost]
        public async Task<IActionResult> BeneficiaryPay(PaymentViewModel paymentView)
        {
            if (!ModelState.IsValid)
            {
                paymentView.AccountsOwn = await _savingAccountService.GetAllViewModelWithInclude();
                return View(paymentView);
            }

            ResponsePaymentViewModel model = await _operationService.PayValidation(paymentView);


            if (model.HasError)
            {
                paymentView.HasError = true;
                paymentView.Error = model.Error;
                paymentView.AccountsOwn = await _savingAccountService.GetAllViewModelWithInclude();
                return View(paymentView);
            }

            return View("ConfirmPay", model);

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

            return View("ConfirmPay", model);
        }

        //Esto podria tener un nombre generico para "ExpressPay y BeneficiaryPay"
        [HttpPost]
        public async Task<IActionResult> ProcessPayment(ResponsePaymentViewModel vm)
        {
            await _operationService.Pay(vm);
            return RedirectToRoute(new { controller = "Client", action = "Index"});
        }
        #endregion

    }
}
