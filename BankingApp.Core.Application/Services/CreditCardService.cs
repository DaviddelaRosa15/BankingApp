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
        public override async Task<SaveCreditCardViewModel> Delete(int id)
        {
            var creditCard = await _creditCardRepository.GetByIdAsync(id);
            SaveCreditCardViewModel card = _mapper.Map<SaveCreditCardViewModel>(creditCard); ;
            if (creditCard.Debit <= 0)
            {
                await base.Delete(id);
                card.HasError = false;
                return card;
            }
            card.HasError = true;
            card.Error = "No se puede eliminar la tarjeta de credito, ya que esta debe";
            return card;

        }
        public override async Task<SaveCreditCardViewModel> Add(SaveCreditCardViewModel vm)
        {
            if (vm.Limit < 500 || vm.Limit > 100000)
            {
                vm.HasError = true;
                vm.Error = "El limite debe estar entre: 500-100000";
                return vm;
            }
            vm.AvailableCredit = vm.Limit;
            await base.Add(vm);
            return vm;
        }
        public async Task<List<SaveCreditCardViewModel>> GetAllCreditCardByIdUser(string id)
        {
            List<CreditCard> credit = await _creditCardRepository.GetAllAsync();
            List<SaveCreditCardViewModel> svm = _mapper.Map<List<SaveCreditCardViewModel>>(credit);

            return svm.Where(svm => svm.UserId == id).ToList();
        }
        public async Task<int> CountCreditCard()
        {
            List<CreditCard> credit = await _creditCardRepository.GetAllAsync();
            return credit.Count();
        }


    }
}
