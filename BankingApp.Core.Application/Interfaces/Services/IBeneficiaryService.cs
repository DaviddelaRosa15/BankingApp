﻿using BankingApp.Core.Application.ViewModels.Beneficiary;
using BankingApp.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingApp.Core.Application.Interfaces.Services
{
    public interface IBeneficiaryService : IGenericService<SaveViewModelBeneficiary, BeneficiaryViewModel, Beneficiary>
    {
        Task<List<BeneficiaryViewModel>> GetAllViewModelWithInclude();
    }
}
