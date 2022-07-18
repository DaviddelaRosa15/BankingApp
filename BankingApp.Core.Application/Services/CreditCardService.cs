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
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly AuthenticationResponse userViewModel;

        public CreditCardService(ICreditCardRepository creditCardRepository, IMapper mapper, IHttpContextAccessor httpContextAccessor) : base(creditCardRepository, mapper)
        {
            _creditCardRepository = creditCardRepository;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
            //this.userViewModel = httpContextAccessor.HttpContext.Session.Get<AuthenticationResponse>("user_session");
        }

        //public async Task<List<CreditCardViewModel>> GetAllViewModelWithInclude()
        //{
        //    var categoryList = await _categoryRepository.GetAllWithIncludeAsync(new List<string> { "Products"});

        //    return categoryList.Select(category => new CategoryViewModel
        //    {
        //        Name = category.Name,
        //        Description = category.Description,
        //        Id= category.Id,
        //        ProductsQuantity = category.Products.Where(product=> product.UserId == userViewModel.Id).Count()
        //    }).ToList();
        //}

    }
}
