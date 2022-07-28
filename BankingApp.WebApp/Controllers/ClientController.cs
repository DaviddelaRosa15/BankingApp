using AutoMapper;
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
        private readonly IMapper _mapper;

        public ClientController(IUserService userService, IOperationService operationService, IHttpContextAccessor httpContextAccessor,
            ICreditCardService creditCardService, ILoanService loanService, ISavingAccountService savingAccountService,
            IBeneficiaryService beneficiaryService, IMapper mapper)
        {
            _userService = userService;
            _operationService = operationService;
            _httpContextAccessor = httpContextAccessor;
            _loggedUser = _httpContextAccessor.HttpContext.Session.Get<AuthenticationResponse>("user_session");
            _creditCardService = creditCardService;
            _loanService = loanService;
            _savingAccountService = savingAccountService;
            _beneficiaryService = beneficiaryService;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            ClientProductsViewModel clientProducts = new ClientProductsViewModel();

            clientProducts.SavingAccounts = await _savingAccountService.GetAllViewModelWithInclude();
            clientProducts.CreditCards = await _creditCardService.GetAllViewModelWithInclude();
            clientProducts.Loans = await _loanService.GetAllViewModelWithInclude();
            return View(viewName: "Index", model: clientProducts);
        }

        #region Mantenimiento de beneficiarios

        public async Task<IActionResult> MyBeneficiaries()
        {
            ViewBag.beneficiaryStatus = new BeneficiaryViewModel() { HasError = false };
            List<BeneficiaryViewModel> beneficiaries = await _beneficiaryService.GetAllViewModelWithInclude();
            return View(viewName: "MyBeneficiary", model: beneficiaries);
        }

        [HttpPost]
        public async Task<IActionResult> MyBeneficiaries(string SavingAccountId = "")
        {

            if (!ValidationHelper.IsValidProductID(SavingAccountId) || string.IsNullOrEmpty(SavingAccountId) || SavingAccountId == null)
            {

                ViewBag.beneficiaryStatus = new BeneficiaryViewModel()
                {
                    HasError = true,
                    Error = "Por favor, introduzca una cuenta valida."
                }; //En caso de que esto no funcione hacer el inverso, enviar el list por un viewBag.

                List<BeneficiaryViewModel> beneficiaries = await _beneficiaryService.GetAllViewModelWithInclude();
                return View(viewName: "MyBeneficiary", model: beneficiaries);

            }

            var account = await _savingAccountService.GetByIdSaveViewModel(int.Parse(SavingAccountId));

            if (account == null)
            {
                ViewBag.beneficiaryStatus = new BeneficiaryViewModel()
                {
                    HasError = true,
                    Error = $"Esta cuenta {SavingAccountId} no existe..."
                };

                List<BeneficiaryViewModel> beneficiaries = await _beneficiaryService.GetAllViewModelWithInclude();
                return View(viewName: "MyBeneficiary", model: beneficiaries);
            }

            await _beneficiaryService.Add(new SaveViewModelBeneficiary()
            {
                SavingAccountId = account.SavingAccountId,
                UserId = _loggedUser.Id
            });

            return RedirectToRoute(new { controller = "Client", action = "MyBeneficiaries" });
        }

        [HttpGet]
        public async Task<IActionResult> BeneficiaryDelete(string id = "")
        {
            List<BeneficiaryViewModel> beneficiaries = await _beneficiaryService.GetAllViewModelWithInclude();
            int identifier = int.Parse(id);

            BeneficiaryViewModel beneficiary = beneficiaries.FirstOrDefault(benf => benf.Id == identifier);

            return View(viewName: "ConfirmBeneficiaryDelete", model: beneficiary);

        }

        [HttpPost]
        public async Task<IActionResult> BeneficiaryDelete(BeneficiaryViewModel beneficiary)
        {
            await _beneficiaryService.Delete(beneficiary.Id);
            return RedirectToRoute(new { controller = "Client", action = "MyBeneficiaries" });
        }

        #endregion

        #region Pago de beneficiarios

        [HttpGet]
        //Route where we are gonna again show all our beneficieries to make a pay.
        public async Task<IActionResult> MyBeneficiariesPay()
        {
            List<BeneficiaryViewModel> beneficiaries = await _beneficiaryService.GetAllViewModelWithInclude();

            return View(viewName: "MyBeneficiariesPay", model: beneficiaries);
        }

        //Intermediario para selecionar las cuenta y enviar el pago.
        [HttpGet]
        public async Task<IActionResult> BeneficiaryPay(int id)
        {
            //Pago para el benecifiario.
            PaymentViewModel model = new();
            model.AccountsOwn = await _savingAccountService.GetAllViewModelWithInclude();
            model.DestinyAccount = id;

            return View(viewName: "BeneficiaryPay", model);

        }

        [HttpPost]
        public async Task<IActionResult> BeneficiaryPay(PaymentViewModel paymentView)
        {
            if (!ModelState.IsValid)
            {
                paymentView.AccountsOwn = await _savingAccountService.GetAllViewModelWithInclude();
                return View("BeneficiaryPay", paymentView);
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

        #endregion

        #region Pago expreso
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

        #endregion

        #region ExpressPay y BeneficiaryPay
        [HttpPost]
        public async Task<IActionResult> ProcessPayment(ResponsePaymentViewModel vm)
        {
            await _operationService.Pay(vm);
            return RedirectToRoute(new { controller = "Client", action = "Index" });
        }
        #endregion

        #region Pago de tarjeta de créditos
        public async Task<IActionResult> CreditCardPay()
        {
            var cards = await _creditCardService.GetAllViewModelWithInclude();
            return View(cards);
        }

        public async Task<IActionResult> MakeCreditCardPay(int id)
        {
            var card = await _creditCardService.GetByIdSaveViewModel(id);
            CreditPaymentViewModel model = new()
            {
                AccountsOwn = await _savingAccountService.GetAllViewModelWithInclude(),
                DestinyCard = card.Id
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> MakeCreditCardPay(CreditPaymentViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                vm.AccountsOwn = await _savingAccountService.GetAllViewModelWithInclude();
                return View(vm);
            }

            CreditPaymentViewModel model = await _operationService.CreditCardPay(vm);

            if (model.HasError)
            {
                vm.HasError = true;
                vm.Error = model.Error;
                vm.AccountsOwn = await _savingAccountService.GetAllViewModelWithInclude();
                return View(vm);
            }

            return RedirectToRoute(new { controller = "Client", action = "Index" });
        }

        #endregion

        #region Pago de prestamos
        public async Task<IActionResult> LoanPay()
        {
            var loans = await _loanService.GetAllViewModelWithInclude();

            return View(loans);
        }
        public async Task<IActionResult> MakeLoanPay(int id)
        {
            var loan = await _loanService.GetByIdSaveViewModel(id);
            LoanPaymentViewModel model = new()
            {
                AccountsOwn = await _savingAccountService.GetAllViewModelWithInclude(),
                Share = loan.Share,
                DestinyLoan = loan.Id
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> MakeLoanPay(LoanPaymentViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                vm.AccountsOwn = await _savingAccountService.GetAllViewModelWithInclude();
                return View(vm);
            }

            LoanPaymentViewModel model = await _operationService.LoanPay(vm);

            if (model.HasError)
            {
                vm.HasError = true;
                vm.Error = model.Error;
                vm.AccountsOwn = await _savingAccountService.GetAllViewModelWithInclude();
                return View(vm);
            }

            return RedirectToRoute(new { controller = "Client", action = "Index" });
        }

        #endregion

        #region Avance de Efectivo
        [HttpGet]
        public async Task<IActionResult> CashAdvancePay()
        {
            CashAdvanceViewModel cashAdvanceView = new();

            cashAdvanceView.OriginCreditCards = await _creditCardService.GetAllViewModelWithInclude();
            cashAdvanceView.AccountsOwn = await _savingAccountService.GetAllViewModelWithInclude();
            return View(viewName: "CashAdvancePay", model: cashAdvanceView);
        }

        [HttpPost]
        public async Task<IActionResult> CashAdvancePay(CashAdvanceViewModel cashAdvanceView)
        {
            if (!ModelState.IsValid)
            {
                cashAdvanceView.AccountsOwn = await _savingAccountService.GetAllViewModelWithInclude();
                cashAdvanceView.OriginCreditCards = await _creditCardService.GetAllViewModelWithInclude();
                return View(viewName: "CashAdvancePay", model: cashAdvanceView);
            }

            var opResult = await _operationService.CashAdvance(cashAdvanceView);

            if (opResult.HasError)
            {
                cashAdvanceView.HasError = true;
                cashAdvanceView.Error = opResult.Error;
                cashAdvanceView.AccountsOwn = await _savingAccountService.GetAllViewModelWithInclude();
                cashAdvanceView.OriginCreditCards = await _creditCardService.GetAllViewModelWithInclude();
                return View(viewName: "CashAdvancePay", model: cashAdvanceView);
            }

            return RedirectToRoute(new { controller = "Client", action = "Index" });

        }
        #endregion

        #region Transferencia entre cuentas
        public async Task<IActionResult> AccountTransfer()
        {
            AccountTransferViewModel model = new();
            model.AccountsOwn = await _savingAccountService.GetAllViewModelWithInclude();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AccountTransfer(AccountTransferViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                vm.AccountsOwn = await _savingAccountService.GetAllViewModelWithInclude();
                return View(vm);
            }

            AccountTransferViewModel model = await _operationService.AccountTransfer(vm);

            if (model.HasError)
            {
                vm.HasError = true;
                vm.Error = model.Error;
                vm.AccountsOwn = await _savingAccountService.GetAllViewModelWithInclude();
                return View(vm);
            }

            return RedirectToRoute(new { controller = "Client", action = "Index" });
        }
        #endregion
    }
}
