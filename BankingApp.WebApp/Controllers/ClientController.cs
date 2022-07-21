using BankingApp.Core.Application.Interfaces.Services;
using BankingApp.Core.Application.ViewModels.Operation;
using BankingApp.WebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace BankingApp.WebApp.Controllers
{
    public class ClientController : Controller
    {

        private readonly IUserService _userService;
        private readonly IOperationService _operationService;
        private readonly ISavingAccountService _savingService;
        public ClientController(IUserService userService, IOperationService operationService, ISavingAccountService savingService)
        {
            _userService = userService;
            _operationService = operationService;
            _savingService = savingService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> ExpressPay()
        {
            PaymentViewModel model = new();
            model.AccountsOwn = await _savingService.GetAllViewModelWithInclude();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ExpressPay(PaymentViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                vm.AccountsOwn = await _savingService.GetAllViewModelWithInclude();
                return View(vm);
            }

            ResponsePaymentViewModel model = await _operationService.PayValidation(vm);

            if (model.HasError)
            {
                vm.HasError = true;
                vm.Error = model.Error;
                vm.AccountsOwn = await _savingService.GetAllViewModelWithInclude();
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
