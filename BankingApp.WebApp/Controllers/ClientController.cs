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

        public ClientController()
        {
            
        }

        public IActionResult Index()
        {
            return View();
        }

    }
}
