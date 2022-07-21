using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BankingApp.Core.Application.Helpers
{
    public static class ValidationHelper
    {

        public static bool IsValidProductID(string identifier)
        {

            bool isValidAccount = new Regex("^[0-9]{9}$", RegexOptions.None)
                                      .IsMatch(identifier);

            return isValidAccount;
        }

    }
}
