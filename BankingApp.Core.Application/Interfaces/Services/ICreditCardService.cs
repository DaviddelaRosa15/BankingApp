using BankingApp.Core.Application.ViewModels.CreditCard;
using BankingApp.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingApp.Core.Application.Interfaces.Services
{
    public interface ICreditCardService : IGenericService<SaveCreditCardViewModel, CreditCardViewModel, CreditCard>
    {
        Task<List<CreditCardViewModel>> GetAllViewModelWithInclude();
    }
}
