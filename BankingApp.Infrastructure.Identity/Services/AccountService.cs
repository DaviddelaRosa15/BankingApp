using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using BankingApp.Core.Application.Dtos.Account;
using BankingApp.Core.Application.Enums;
using BankingApp.Core.Application.Interfaces.Services;
using BankingApp.Infrastructure.Identity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BankingApp.Core.Application.ViewModels.Client;
using BankingApp.Core.Application.ViewModels.User;
using Microsoft.EntityFrameworkCore;
using BankingApp.Core.Application.ViewModels.SavingAccount;

namespace BankingApp.Infrastructure.Identity.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailService _emailService;
        private readonly ISavingAccountService _savingAccountService;

        public AccountService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, 
            IEmailService emailService, ISavingAccountService savingAccountService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailService = emailService;
            _savingAccountService = savingAccountService;
        }

        #region Custom method
        public async Task<CountClient> CountClient()
        {
            CountClient countClient = new();
            var clients = await _userManager.GetUsersInRoleAsync("Client");
            foreach (ApplicationUser user in clients)
            {
                if (user.EmailConfirmed)
                {
                    countClient.ActiveTotal += 1;
                }
                else
                {
                    countClient.DesactiveTotal += 1;
                }
            }
            return countClient;
        }

        public async Task<List<SaveUserViewModel>> GetAllUserAdminAsync()
        {
            var users = await _userManager.GetUsersInRoleAsync("Administrator");
            List<SaveUserViewModel> svm = new();
            if (users != null)
            {
                foreach (var user in users)
                {
                    svm.Add(new SaveUserViewModel()
                    {
                        Id = user.Id,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        CardIdentificantion = user.CardIdentification,
                        Email = user.Email,
                        ConfirEmail = user.EmailConfirmed,
                        Username = user.UserName
                    });
                }
            }
            return svm;
        }

        public async Task<List<SaveUserViewModel>> GetAllUserClientAsync()
        {
            var users = await _userManager.GetUsersInRoleAsync("Client");
            List<SaveUserViewModel> svm = new();
            if (users != null)
            {
                foreach (var user in users)
                {
                    svm.Add(new SaveUserViewModel()
                    {
                        Id = user.Id,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        CardIdentificantion = user.CardIdentification,
                        Email = user.Email,
                        ConfirEmail = user.EmailConfirmed,
                        Username = user.UserName
                    });
                }
            }
            return svm;
        }
        public async Task<SaveUserViewModel> GetUserByIdAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            SaveUserViewModel svM = new();
            if (user != null)
            {
                svM = new()
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    CardIdentificantion = user.CardIdentification,
                    Email = user.Email,
                    ConfirEmail = user.EmailConfirmed,
                    Username = user.UserName
                };
                return svM;
            }
            svM.Error = "Usuario no encontrado";
            svM.HasError = true;
            return svM;
        }

        public async Task<SaveUserViewModel> UpdateUserAsync(SaveUserViewModel svm)
        {
            ApplicationUser appliUser = await _userManager.FindByIdAsync(svm.Id);
            SaveUserViewModel sv = new();
            var user = await _userManager.Users.ToListAsync();
            if (appliUser.UserName != svm.Username)
            {
                var verifUsername = await _userManager.FindByNameAsync(svm.Username);
                if (verifUsername != null)
                {
                    sv.HasError = true;
                    sv.Error = $"Este usuario {svm.Username} ya esta en uso";
                    return sv;
                }
            }
            if (appliUser.CardIdentification != svm.CardIdentificantion)
            {
                try
                {
                    var verifyCedula = user.FirstOrDefault(user => user.CardIdentification == svm.CardIdentificantion);
                    if (verifyCedula != null)
                    {
                        sv.HasError = true;
                        sv.Error = $"Esta cedula {svm.CardIdentificantion} ya esta en uso";
                        return sv;
                    }
                }
                catch (InvalidOperationException ex)
                {
                    sv.HasError = true;
                    sv.Error = $"Esta cedula {svm.CardIdentificantion} ya esta en uso";
                    return sv;
                }

            }
            if (appliUser.Email != svm.Email)
            {
                try
                {
                    var verifyEmail = await _userManager.FindByEmailAsync(svm.Email);
                    if (verifyEmail != null)
                    {
                        sv.HasError = true;
                        sv.Error = $"Este email {svm.Email} ya esta en uso";
                        return sv;
                    }
                }
                catch (InvalidOperationException ex)
                {
                    sv.HasError = true;
                    sv.Error = $"Este email {svm.Email} ya esta en uso";
                    return sv;
                }


            }


            appliUser.FirstName = svm.FirstName;
            appliUser.LastName = svm.LastName;
            appliUser.Email = svm.Email;
            appliUser.CardIdentification = svm.CardIdentificantion;

            if (svm.TypeUser == "Cliente")
            {

                if (svm.Amount > 0)
                {
                    var vm = await _savingAccountService.GetCardByIdUserAsync(svm.Id);
                    vm.Balance += svm.AditionalAmount;
                    await _savingAccountService.Update(vm, vm.SavingAccountId);
                }

            }

            if (svm.Password != null)
            {
                var statusUpdate = await _userManager.ChangePasswordAsync(appliUser, svm.CurrentPassword, svm.Password);
                if (statusUpdate.Succeeded)
                {
                    sv.HasError = false;
                }
                else
                {
                    sv.HasError = true;
                    foreach (var error in statusUpdate.Errors)
                    {
                        if (error.Code == "PasswordMismatch")
                        {
                            sv.Error += "La contraseña actual es incorrecta";
                        }
                        else
                        {
                            sv.Error += error.Code;
                        }

                    }
                }
            }
            if (appliUser.UserName != svm.Username)
            {
                var userName = await _userManager.FindByNameAsync(svm.Username);
                if (userName == null)
                {
                    appliUser.UserName = svm.Username;
                }
                else
                {
                    sv.HasError = true;
                    sv.Error = "El nombre de usuario ya existe!";
                }
            }
            await _userManager.UpdateAsync(appliUser);
            return sv;
        }

        public async Task<SaveUserViewModel> CreateUser(SaveUserViewModel svm)
        {
            var user = await _userManager.Users.ToListAsync();
            SaveUserViewModel sv = new();
            var verifUsername = await _userManager.FindByNameAsync(svm.Username);
            if (verifUsername != null)
            {
                sv.HasError = true;
                sv.Error = $"Este usuario {svm.Username} ya esta en uso";
                return sv;
            }
            try
            {
                var verifyCedula = user.FirstOrDefault(user => user.CardIdentification == svm.CardIdentificantion);
                if (verifyCedula != null)
                {
                    sv.HasError = true;
                    sv.Error = $"Esta cedula {svm.CardIdentificantion} ya esta en uso";
                    return sv;
                }
            }
            catch (InvalidOperationException ex)
            {
                sv.HasError = true;
                sv.Error = $"Esta cedula {svm.CardIdentificantion} ya esta en uso";
                return sv;
            }

            try
            {
                var verifyEmail = await _userManager.FindByEmailAsync(svm.Email);
                if (verifyEmail != null)
                {
                    sv.HasError = true;
                    sv.Error = $"Este email {svm.Email} ya esta en uso";
                    return sv;
                }
            }
            catch (InvalidOperationException ex)
            {
                sv.HasError = true;
                sv.Error = $"Este email {svm.Email} ya esta en uso";
                return sv;
            }
            sv.HasError = true;
            ApplicationUser appliUser = new()
            {
                FirstName = svm.FirstName,
                LastName = svm.LastName,
                Email = svm.Email,
                CardIdentification = svm.CardIdentificantion,
                UserName = svm.Username,
                EmailConfirmed = true
            };
            var status = await _userManager.CreateAsync(appliUser, svm.Password);
            if (status.Succeeded)
            {
                sv.HasError = false;
                if (svm.TypeUser == "Administrador")
                {
                    await _userManager.AddToRoleAsync(appliUser, Roles.Administrator.ToString());
                }
                if (svm.TypeUser == "Cliente")
                {
                    SaveVM_SavingAccount svA = new();
                    svA.UserId = appliUser.Id;
                    svA.IsPrincipal = true;
                    svA.Balance = svm.Amount;
                    await _savingAccountService.Add(svA);
                    await _userManager.AddToRoleAsync(appliUser, Roles.Client.ToString());
                }
                return sv;
            }
            else
            {
                sv.HasError = true;
                foreach (var error in status.Errors)
                {
                    sv.Error += error.Code;
                }
                return sv;
            }
        }

        public async Task DesactiveUser(string id)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                user.EmailConfirmed = false;
                await _userManager.UpdateAsync(user);
            }

        }
        public async Task ActiveUser(string id)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                user.EmailConfirmed = true;
                await _userManager.UpdateAsync(user);
            }
        }

        #endregion


        public async Task<AuthenticationResponse> AuthenticateAsync(AuthenticationRequest request)
        {
            AuthenticationResponse response = new();

            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                response.HasError = true;
                response.Error = $"No Accounts registered with {request.Email}";
                return response;
            }

            var result = await _signInManager.PasswordSignInAsync(user.UserName, request.Password, false, lockoutOnFailure: false);
            if (!result.Succeeded)
            {
                response.HasError = true;
                response.Error = $"Invalid credentials for {request.Email}";
                return response;
            }
            if (!user.EmailConfirmed)
            {
                response.HasError = true;
                response.Error = $"Account no confirmed for {request.Email}";
                return response;
            }

            response.Id = user.Id;
            response.Email = user.Email;
            response.UserName = user.UserName;

            var rolesList = await _userManager.GetRolesAsync(user).ConfigureAwait(false);

            response.Roles = rolesList.ToList();
            response.IsVerified = user.EmailConfirmed;

            return response;
        }

        public async Task SignOutAsync()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<RegisterResponse> RegisterBasicUserAsync(RegisterRequest request, string origin)
        {
            RegisterResponse response = new()
            {
                HasError = false
            };

            var userWithSameUserName = await _userManager.FindByNameAsync(request.UserName);
            if (userWithSameUserName != null)
            {
                response.HasError = true;
                response.Error = $"username '{request.UserName}' is already taken.";
                return response;
            }

            var userWithSameEmail = await _userManager.FindByEmailAsync(request.Email);
            if (userWithSameEmail != null)
            {
                response.HasError = true;
                response.Error = $"Email '{request.Email}' is already registered.";
                return response;
            }

            var user = new ApplicationUser
            {
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                CardIdentification = request.CardIdentification,
                UserName = request.UserName
            };

            var result = await _userManager.CreateAsync(user, request.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, Roles.Client.ToString());
                var verificationUri = await SendVerificationEmailUri(user, origin);
                await _emailService.SendAsync(new Core.Application.DTOs.Email.EmailRequest()
                {
                    To = user.Email,
                    Body = $"Please confirm your account visiting this URL {verificationUri}",
                    Subject = "Confirm registration"
                });
            }
            else
            {
                response.HasError = true;
                response.Error = $"An error occurred trying to register the user.";
                return response;
            }

            return response;
        }

        public async Task<string> ConfirmAccountAsync(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return $"No accounts registered with this user";
            }

            token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));
            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                return $"Account confirmed for {user.Email}. You can now use the app";
            }
            else
            {
                return $"An error occurred wgile confirming {user.Email}.";
            }
        }

        public async Task<ForgotPasswordResponse> ForgotPasswordAsync(ForgotPasswordRequest request, string origin)
        {
            ForgotPasswordResponse response = new()
            {
                HasError = false
            };

            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user == null)
            {
                response.HasError = true;
                response.Error = $"No Accounts registered with {request.Email}";
                return response;
            }

            var verificationUri = await SendForgotPasswordUri(user, origin);

            await _emailService.SendAsync(new Core.Application.DTOs.Email.EmailRequest()
            {
                To = user.Email,
                Body = $"Please reset your account visiting this URL {verificationUri}",
                Subject = "reset password"
            });


            return response;
        }

        public async Task<AuthenticationResponse> GetUserById(string id)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(id);
            AuthenticationResponse response = new()
            {
                Id = user.Id,
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                CardIdentification = user.CardIdentification,
                Email = user.Email
            };
            return response;
        }
        public async Task<ResetPasswordResponse> ResetPasswordAsync(ResetPasswordRequest request)
        {
            ResetPasswordResponse response = new()
            {
                HasError = false
            };

            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user == null)
            {
                response.HasError = true;
                response.Error = $"No Accounts registered with {request.Email}";
                return response;
            }

            request.Token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.Token));
            var result = await _userManager.ResetPasswordAsync(user, request.Token, request.Password);

            if (!result.Succeeded)
            {
                response.HasError = true;
                response.Error = $"An error occurred while reset password";
                return response;
            }

            return response;
        }
        private async Task<string> SendVerificationEmailUri(ApplicationUser user, string origin)
        {
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var route = "User/ConfirmEmail";
            var Uri = new Uri(string.Concat($"{origin}/", route));
            var verificationUri = QueryHelpers.AddQueryString(Uri.ToString(), "userId", user.Id);
            verificationUri = QueryHelpers.AddQueryString(verificationUri, "token", code);

            return verificationUri;
        }
        private async Task<string> SendForgotPasswordUri(ApplicationUser user, string origin)
        {
            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var route = "User/ResetPassword";
            var Uri = new Uri(string.Concat($"{origin}/", route));
            var verificationUri = QueryHelpers.AddQueryString(Uri.ToString(), "token", code);

            return verificationUri;
        }
    }


}
