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
        private readonly ISavingAccountService _savingService;
        private readonly ICreditCardService _cardService;

        public ClientController(IUserService userService, IOperationService operationService, ISavingAccountService savingService, ICreditCardService cardService)
        {
            _userService = userService;
            _operationService = operationService;
            _savingService = savingService;
            _cardService = cardService;
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
            return RedirectToRoute(new { controller = "Client", action = "Index" });
        }

        public async Task<IActionResult> CreditCardPay()
        {
            CreditPaymentViewModel model = new();
            model.AccountsOwn = await _savingService.GetAllViewModelWithInclude();
            model.CardsOwn = await _cardService.GetAllViewModel();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CreditCardPay(CreditPaymentViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                vm.AccountsOwn = await _savingService.GetAllViewModelWithInclude();
                vm.CardsOwn = await _cardService.GetAllViewModelWithInclude();
                return View(vm);
            }

            CreditPaymentViewModel model = await _operationService.CreditCardPay(vm);

            if (model.HasError)
            {
                vm.HasError = true;
                vm.Error = model.Error;
                vm.AccountsOwn = await _savingService.GetAllViewModelWithInclude();
                vm.CardsOwn = await _cardService.GetAllViewModelWithInclude();
                return View(vm);
            }

            return RedirectToRoute(new { controller = "Client", action = "Index" });
        }
        #endregion

    }
}
