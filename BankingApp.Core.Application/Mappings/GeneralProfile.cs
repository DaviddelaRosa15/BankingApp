using AutoMapper;
using BankingApp.Core.Application.Dtos.Account;
using BankingApp.Core.Application.ViewModels.Beneficiary;
using BankingApp.Core.Application.ViewModels.CreditCard;
using BankingApp.Core.Application.ViewModels.Loan;
using BankingApp.Core.Application.ViewModels.SavingAccount;
using BankingApp.Core.Application.ViewModels.Transactions;
using BankingApp.Core.Application.ViewModels.User;
using BankingApp.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace BankingApp.Core.Application.Mappings
{
    public class GeneralProfile : Profile
    {
        public GeneralProfile()
        {
            #region User
            CreateMap<AuthenticationRequest, LoginViewModel>()
                .ForMember(x => x.HasError, opt => opt.Ignore())
                .ForMember(x => x.Error, opt => opt.Ignore())
                .ReverseMap();

            CreateMap<RegisterRequest, SaveUserViewModel>()
                .ForMember(x => x.HasError, opt => opt.Ignore())
                .ForMember(x => x.Error, opt => opt.Ignore())
                .ReverseMap();

            CreateMap<ForgotPasswordRequest, ForgotPasswordViewModel>()
                .ForMember(x => x.HasError, opt => opt.Ignore())
                .ForMember(x => x.Error, opt => opt.Ignore())
                .ReverseMap();

            CreateMap<ResetPasswordRequest, ResetPasswordViewModel>()
                .ForMember(x => x.HasError, opt => opt.Ignore())
                .ForMember(x => x.Error, opt => opt.Ignore())
                .ReverseMap();
            #endregion

            #region CreditCard
            CreateMap<CreditCard, CreditCardViewModel>()
                .ReverseMap()
                .ForMember(x => x.Created, opt => opt.Ignore())
                .ForMember(x => x.CreatedBy, opt => opt.Ignore())
                .ForMember(x => x.LastModified, opt => opt.Ignore())
                .ForMember(x => x.LastModifiedBy, opt => opt.Ignore());

            CreateMap<CreditCard, SaveCreditCardViewModel>()
                .ReverseMap()
                .ForMember(x => x.Created, opt => opt.Ignore())
                .ForMember(x => x.CreatedBy, opt => opt.Ignore())
                .ForMember(x => x.LastModified, opt => opt.Ignore())
                .ForMember(x => x.LastModifiedBy, opt => opt.Ignore());
            #endregion

            #region Loan
            CreateMap<Loan, LoanViewModel>()
                .ReverseMap()
                .ForMember(x => x.Created, opt => opt.Ignore())
                .ForMember(x => x.CreatedBy, opt => opt.Ignore())
                .ForMember(x => x.LastModified, opt => opt.Ignore())
                .ForMember(x => x.LastModifiedBy, opt => opt.Ignore());

            CreateMap<Loan, SaveLoanViewModel>()
                .ReverseMap()
                .ForMember(x => x.Created, opt => opt.Ignore())
                .ForMember(x => x.CreatedBy, opt => opt.Ignore())
                .ForMember(x => x.LastModified, opt => opt.Ignore())
                .ForMember(x => x.LastModifiedBy, opt => opt.Ignore());
            #endregion


            #region Saving_Account
            CreateMap<SavingAccount, SavingAccountViewModel>()
                .ForMember(dest => dest.SavingAccountId, opt => opt.MapFrom(src=> src.Id))
                .ReverseMap()
                 .ForMember(source => source.Id, opt=> opt.MapFrom(dest => dest.SavingAccountId))
                 .ForMember(sour => sour.Created, option => option.Ignore())
                 .ForMember(sour => sour.CreatedBy, option => option.Ignore())
                 .ForMember(sour => sour.LastModified, option => option.Ignore())
                 .ForMember(sour => sour.LastModifiedBy, option => option.Ignore());


            CreateMap<SavingAccount, SaveVM_SavingAccount>()
                .ForMember(dest => dest.SavingAccountId, opt => opt.MapFrom(src => src.Id))
                .ReverseMap()
                    .ForMember(source => source.Id, opt => opt.MapFrom(dest => dest.SavingAccountId))
                    .ForMember(source => source.Created, opt => opt.Ignore())
                    .ForMember(source => source.CreatedBy, opt => opt.Ignore())
                    .ForMember(source => source.LastModified, opt => opt.Ignore())
                    .ForMember(source => source.LastModifiedBy, opt => opt.Ignore())
                    .ForMember(source => source.beneficiaries, opt => opt.Ignore());

            #endregion

            #region Beneficiary
                CreateMap<Beneficiary, SaveViewModelBeneficiary>()
                    .ReverseMap()
                        .ForMember(source => source.Created, opt => opt.Ignore())
                        .ForMember(source => source.CreatedBy, opt => opt.Ignore())
                        .ForMember(source => source.LastModified, opt => opt.Ignore())
                        .ForMember(source => source.LastModifiedBy, opt => opt.Ignore())
                        .ForMember(source => source.SavingAccount, opt => opt.Ignore());


                CreateMap<Beneficiary, BeneficiaryViewModel>()
                    .ReverseMap()
                        .ForMember(source => source.Created, opt => opt.Ignore())
                        .ForMember(source => source.CreatedBy, opt => opt.Ignore())
                        .ForMember(source => source.LastModified, opt => opt.Ignore())
                        .ForMember(source => source.LastModifiedBy, opt => opt.Ignore());
            #endregion

            #region Transaction
            CreateMap<Transaction, SaveViewModelTransaction>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.Created))
                .ReverseMap()
                    .ForMember(source => source.Created, opt => opt.MapFrom(dest => dest.CreatedAt))
                    .ForMember(source => source.CreatedBy, opt => opt.Ignore())
                    .ForMember(source => source.LastModified, opt => opt.Ignore())
                    .ForMember(source => source.LastModifiedBy, opt => opt.Ignore());
            


            CreateMap<Transaction, TransactionViewModel>()
                .ForMember(dest => dest.CreatedAt, opt=> opt.MapFrom(src => src.Created)) 
                .ReverseMap()
                    .ForMember(source => source.Created, opt => opt.MapFrom(dest=> dest.CreatedAt))
                    .ForMember(source => source.CreatedBy, opt => opt.Ignore())
                    .ForMember(source => source.LastModified, opt => opt.Ignore())
                    .ForMember(source => source.LastModifiedBy, opt => opt.Ignore());
            #endregion


        }
    }
}
