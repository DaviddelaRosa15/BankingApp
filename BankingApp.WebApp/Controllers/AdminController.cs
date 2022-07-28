using BankingApp.Core.Application.Interfaces.Services;
using BankingApp.Core.Application.ViewModels.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BankingApp.WebApp.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class AdminController : Controller
    {
        private readonly IUserService _userService;
        private readonly IAccountService _accountService;
        private readonly ITransactionService _transactionService;
        private readonly ILoanService _loanService;
        private readonly ICreditCardService _creditCardService;
        private readonly IProductClient _productClient;

        public AdminController(IUserService userService, ITransactionService transactionService,
            ILoanService loanService, IAccountService accountService, ICreditCardService creditCardService,
            IProductClient productClient
        )
        {
            _userService = userService;
            _loanService = loanService;
            _transactionService = transactionService;
            _accountService = accountService;
            _creditCardService = creditCardService;
            _productClient = productClient;
        }
        public async Task<IActionResult> Index()
        {
            ViewBag.CountTransaction = await _transactionService.CountTransaction();
            ViewBag.CountPay = await _transactionService.CountPay();
            ViewBag.CountClient = await _accountService.CountClient();
            ViewBag.CountProductAsigned = await _productClient.CountProductAsigned();
            return View(await _userService.GetAllUserAdminAsync());
        }

        public async Task<IActionResult> Admin()
        {
            return View(await _userService.GetAllUserAdminAsync());
        }

        public IActionResult Create()
        {
            return View(new SaveUserViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Create(SaveUserViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }
            SaveUserViewModel saveStatus = await _userService.CreateUser(vm);
            if (saveStatus.HasError)
            {
                vm.HasError = true;
                vm.Error = saveStatus.Error;
                return View(vm);
            }
            return RedirectToRoute(new { controller = "Admin", action = "Admin" });
        }

        public async Task<IActionResult> EditAdmin(string id)
        {
            return View(await _userService.GetUserByIdAsync(id));
        }

        [HttpPost]
        public async Task<IActionResult> EditAdmin(SaveUserViewModel svm)
        {
            if (ModelState["Email"].Errors.Any() || ModelState["FirstName"].Errors.Any()
                || ModelState["LastName"].Errors.Any() || ModelState["CardIdentificantion"].Errors.Any()
                || ModelState["Username"].Errors.Any())
            {
                return View(svm);
            }
            SaveUserViewModel updateStatus = await _userService.UpdateUserAsync(svm);
            if (updateStatus.HasError)
            {
                svm.HasError = updateStatus.HasError;
                svm.Error = updateStatus.Error;
                return View(svm);
            }
            return RedirectToRoute(new { controller = "Admin", action = "Admin" });
        }

        public async Task<IActionResult> Desactive(string id)
        {
            await _userService.DesactiveUser(id);
            return RedirectToRoute(new { controller = "Admin", action = "Admin" });
        }
        public async Task<IActionResult> Active(string id)
        {
            await _userService.ActiveUser(id);
            return RedirectToRoute(new { controller = "Admin", action = "Admin" });
        }
    }
}
