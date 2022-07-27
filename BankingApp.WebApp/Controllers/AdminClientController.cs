using BankingApp.Core.Application.Interfaces.Repositories;
using BankingApp.Core.Application.Interfaces.Services;
using BankingApp.Core.Application.ViewModels.CreditCard;
using BankingApp.Core.Application.ViewModels.Loan;
using BankingApp.Core.Application.ViewModels.SavingAccount;
using BankingApp.Core.Application.ViewModels.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace BankingApp.WebApp.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class AdminClientController : Controller
    {
        private readonly ICreditCardService _cardService;
        private readonly IUserService _userService;
        private readonly ILoanService _loanService;
        private readonly ISavingAccountService _savingAccountService;
        public AdminClientController(IUserService userService, ICreditCardService cardService, ISavingAccountService savingAccountRepository, ILoanService loanService)
        {
            _userService = userService;
            _loanService = loanService;
            _savingAccountService = savingAccountRepository;
            _cardService = cardService;
        }
        public async Task<IActionResult> Index()
        {
            return View(await _userService.GetAllUserClientAsync());
        }

        public async Task<IActionResult> Edit(string id,string error = null)
        {
            ViewBag.Error = error;
            ViewBag.CreditCardClient = await _cardService.GetAllCreditCardByIdUser(id);
            ViewBag.SavingAccount = await _savingAccountService.GetAllAccountByIdUser(id);
            ViewBag.Loan = await _loanService.GetAllLoanByIdUser(id);
            return View(await _userService.GetUserByIdAsync(id));
        }

        [HttpPost]
        public async Task<IActionResult> Edit(SaveUserViewModel svm)
        {            
            if (ModelState["Email"].Errors.Any() || ModelState["FirstName"].Errors.Any()
                || ModelState["LastName"].Errors.Any() || ModelState["CardIdentificantion"].Errors.Any()
                || ModelState["Username"].Errors.Any())
            {
                ViewBag.CreditCardClient = await _cardService.GetAllCreditCardByIdUser(svm.Id);
                ViewBag.SavingAccount = await _savingAccountService.GetAllAccountByIdUser(svm.Id);
                ViewBag.Loan = await _loanService.GetAllLoanByIdUser(svm.Id);
                return View(svm);
            }
            if (svm.Password != svm.ConfirmPassword)
            {
                svm.HasError = true;
                svm.Error = "La contraseña nueva no coinciden";
                ViewBag.CreditCardClient = await _cardService.GetAllCreditCardByIdUser(svm.Id);
                ViewBag.SavingAccount = await _savingAccountService.GetAllAccountByIdUser(svm.Id);
                ViewBag.Loan = await _loanService.GetAllLoanByIdUser(svm.Id);
                return View(svm);
            }
            svm.TypeUser = "Cliente";
            SaveUserViewModel updateStatus = await _userService.UpdateUserAsync(svm);
            if (updateStatus.HasError)
            {
                svm.HasError = updateStatus.HasError;
                svm.Error = updateStatus.Error;
                return View(svm);
            }
            return RedirectToRoute(new { controller = "AdminClient", action = "Index" });
        }
        public async Task<IActionResult> Desactive(string id)
        {
            await _userService.DesactiveUser(id);
            return RedirectToRoute(new { controller = "AdminClient", action = "Index" });
        }
        public async Task<IActionResult> Active(string id)
        {
            await _userService.ActiveUser(id);
            return RedirectToRoute(new { controller = "AdminClient", action = "Index" });
        }
    #region Credit card
        public async Task<IActionResult> CreatedCard(string id)
    {
        ViewBag.Id = id;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> CreatedCard(SaveCreditCardViewModel svm)
    {
      if (svm.Limit < 500 || svm.Limit > 100000)
      {
          ViewBag.Id = svm.UserId;
          svm.HasError = true;
          svm.Error = "El limite debe estar entre: 500-100000";
          return View(svm);
      }
      await _cardService.Add(svm);
      return RedirectToRoute(new { controller = "AdminClient", action = "Edit", id = svm.UserId });
    }
    public async Task<IActionResult> DeleteCreditCard(int id)
    {
         var card = await _cardService.Delete(id);
         if (card.HasError)
         {
            return RedirectToRoute(new { controller = "AdminClient", action = "Edit", id = card.UserId, error = card.Error });
         }
         return RedirectToRoute(new { controller = "AdminClient", action = "Edit", id = card.UserId });
    }
        #endregion

        #region SavingAccount
        public async Task<IActionResult> CreatedSavingAccount(string id)
        {
            ViewBag.Id = id;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreatedSavingAccount(SaveVM_SavingAccount svm)
        {
            if (svm.Balance < 499 )
            {
                ViewBag.Id = svm.UserId;
                svm.HasError = true;
                svm.Error = "El balance debe ser mayor o igual a: $500";
                return View(svm);
            }
            await _savingAccountService.Add(svm);
            return RedirectToRoute(new { controller = "AdminClient", action = "Edit", id = svm.UserId });
        }
        public async Task<IActionResult> DeleteSavingAccount(int id)
        {
            var account = await _savingAccountService.Delete(id);
            return RedirectToRoute(new { controller = "AdminClient", action = "Edit", id = account.UserId });
        }
        #endregion

        #region Loan
        public async Task<IActionResult> CreatedLoan(string id)
        {
            ViewBag.Id = id;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreatedLoan(SaveLoanViewModel svm)
        {
            if (svm.LoanAmount < 500  || svm.LoanAmount>150000)
            {
                ViewBag.Id = svm.UserId;
                svm.HasError = true;
                svm.Error = "El monto debe estar entre: 500-15000";
                return View(svm);
            }
           var stauts= await _loanService.Add(svm);
            if (stauts.HasError)
            {
                svm.HasError = true;
                svm.Error = stauts.Error;
                return View(svm);
            }
            return RedirectToRoute(new { controller = "AdminClient", action = "Edit", id = svm.UserId });
        }
        public async Task<IActionResult> DeleteLoan(int id)
        {
            var loan = await _loanService.Delete(id);
            if (loan.HasError)
            {
                return RedirectToRoute(new { controller = "AdminClient", action = "Edit", id = loan.UserId, error = loan.Error });
            }
            return RedirectToRoute(new { controller = "AdminClient", action = "Edit", id = loan.UserId });
        }
        #endregion





    }
}
