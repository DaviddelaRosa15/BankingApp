﻿using AutoMapper;
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
    public class  BeneficiaryService : GenericService<SaveViewModelBeneficiary, BeneficiaryViewModel,Beneficiary> ,IBeneficiaryService
    {

        private readonly IBeneficiaryRepository _beneficiaryRepository;
        private readonly IMapper _mapper;
        private readonly AuthenticationResponse _userViewModel;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public BeneficiaryService(IBeneficiaryRepository beneficiaryRepository, IMapper mapper, IHttpContextAccessor httpContextAccessor, AuthenticationResponse userViewModel)
        : base(beneficiaryRepository, mapper)
        {
            _beneficiaryRepository = beneficiaryRepository;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            this._userViewModel = httpContextAccessor.HttpContext.Session.Get<AuthenticationResponse>("user_session");
        }

        public async Task<List<BeneficiaryViewModel>> GetAllViewModelWithInclude()
        {

            List<Beneficiary> result = await _beneficiaryRepository.GetAllWithIncludeAsync(new List<string>(){ "SavingAccount" });
            return _mapper.Map<List<BeneficiaryViewModel>>(result);

        }

    }
}
