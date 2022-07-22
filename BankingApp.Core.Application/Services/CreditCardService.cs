using AutoMapper;
using Microsoft.AspNetCore.Http;
using BankingApp.Core.Application.Dtos.Account;
using BankingApp.Core.Application.Helpers;
using BankingApp.Core.Application.Interfaces.Repositories;
using BankingApp.Core.Application.Interfaces.Services;
using BankingApp.Core.Application.ViewModels.User;
using BankingApp.Core.Domain.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BankingApp.Core.Application.Services;
using BankingApp.Core.Application.ViewModels.CreditCard;

namespace BankingApp.Core.Application.Services
{
    public class CreditCardService : GenericService<SaveCreditCardViewModel, CreditCardViewModel, CreditCard>, ICreditCardService
    {
        private readonly ICreditCardRepository _creditCardRepository;
        private readonly IMapper _mapper;
        private readonly AuthenticationResponse userViewModel;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CreditCardService(ICreditCardRepository creditCardRepository, IMapper mapper, IHttpContextAccessor httpContextAccessor) : base(creditCardRepository, mapper)
        {
            _creditCardRepository = creditCardRepository;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            userViewModel = _httpContextAccessor.HttpContext.Session.Get<AuthenticationResponse>("user_session");
        }

        public async Task<List<CreditCardViewModel>> GetAllViewModelWithInclude()
        {
            var cardList = await _creditCardRepository.GetAllWithIncludeAsync(new List<string> { });

            return cardList.Where(x => x.UserId == userViewModel.Id).Select(card => new CreditCardViewModel
            {
                Id = card.Id,
                Debit = card.Debit,
                AvailableCredit = card.AvailableCredit,
                Limit = card.Limit,
                UserId = card.UserId
            }).ToList();
        }

    }
}
