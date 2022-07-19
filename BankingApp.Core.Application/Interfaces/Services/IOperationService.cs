using BankingApp.Core.Application.ViewModels.Operation;
using BankingApp.Core.Application.ViewModels.Transactions;
using BankingApp.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingApp.Core.Application.Interfaces.Services
{
    public interface IOperationService
    {
        Task<ResponseExpressPaymentViewModel> ExpressPay(ExpressPaymentViewModel vm);
        Task<CreditPaymentViewModel> CreditCardPay(CreditPaymentViewModel vm);
    }
}
