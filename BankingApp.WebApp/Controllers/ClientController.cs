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
        public ClientController(IUserService userService, IOperationService operationService)
        {
            this._userService = userService;
            this._operationService = operationService;
        }

        public IActionResult Index()
        {
            return View();
        }

       


    }
}
