﻿using BankingApp.Core.Application.Dtos.Account;
using BankingApp.Core.Application.ViewModels.User;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BankingApp.Core.Application.Interfaces.Services
{
    public interface IUserService
    {
        Task<string> ConfirmEmailAsync(string userId, string token);
        Task<ForgotPasswordResponse> ForgotPasswordAsync(ForgotPasswordViewModel vm, string origin);
        Task<AuthenticationResponse> LoginAsync(LoginViewModel vm);
        Task<RegisterResponse> RegisterAsync(SaveUserViewModel vm, string origin);
        Task<ResetPasswordResponse> ResetPasswordAsync(ResetPasswordViewModel vm);
        Task SignOutAsync();
        Task<AuthenticationResponse> GetUserById(string id);
        Task<SaveUserViewModel> GetUserByIdAsync(string id);
        Task<List<SaveUserViewModel>> GetAllUserAdminAsync();
        Task<List<SaveUserViewModel>> GetAllUserClientAsync();
        Task<SaveUserViewModel> UpdateUserAsync(SaveUserViewModel svm);
        Task<SaveUserViewModel> CreateUser(SaveUserViewModel svm);
        Task DesactiveUser(string id);
        Task ActiveUser(string id);
    }
}