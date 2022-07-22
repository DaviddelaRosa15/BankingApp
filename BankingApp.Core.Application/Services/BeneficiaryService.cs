using AutoMapper;
using BankingApp.Core.Application.Dtos.Account;
using BankingApp.Core.Application.Helpers;
using BankingApp.Core.Application.Interfaces.Repositories;
using BankingApp.Core.Application.Interfaces.Services;
using BankingApp.Core.Application.ViewModels.Beneficiary;
using BankingApp.Core.Domain.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingApp.Core.Application.Services
{
    public class BeneficiaryService : GenericService<SaveViewModelBeneficiary, BeneficiaryViewModel, Beneficiary>, IBeneficiaryService
    {

        private readonly IBeneficiaryRepository _beneficiaryRepository;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;
        private readonly AuthenticationResponse userViewModel;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public BeneficiaryService(IBeneficiaryRepository beneficiaryRepository, IMapper mapper, IUserService userService, IHttpContextAccessor httpContextAccessor)
        : base(beneficiaryRepository, mapper)
        {
            _beneficiaryRepository = beneficiaryRepository;
            _mapper = mapper;
            _userService = userService;
            _httpContextAccessor = httpContextAccessor;
            userViewModel = _httpContextAccessor.HttpContext.Session.Get<AuthenticationResponse>("user_session");
        }

        public async Task<List<BeneficiaryViewModel>> GetAllViewModelWithInclude()
        {

            List<Beneficiary> result = await _beneficiaryRepository.GetAllWithIncludeAsync(new List<string>() { "SavingAccount" });
            
            return result.Where(x => x.UserId == userViewModel.Id).Select(ben => new BeneficiaryViewModel()
            {
                BeneficiaryName = _userService.GetUserById(ben.SavingAccount.UserId).Result.FirstName,
                BeneficiaryLastName = _userService.GetUserById(ben.SavingAccount.UserId).Result.LastName,
                Id = ben.Id,
                SavingAccountId = ben.SavingAccountId
            }).ToList();

        }

    }
}
