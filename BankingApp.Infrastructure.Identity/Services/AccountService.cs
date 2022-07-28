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
using BankingApp.Core.Application.DTOs.Email;

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

            if (svm.Password != svm.ConfirmPassword)
            {
                sv.HasError = true;
                sv.Error = "Las contraseñas nuevas no coinciden";
                return sv;
            }
            if (svm.Password != null)
            {
                if (svm.CurrentPassword == null)
                {
                    sv.HasError = true;
                    sv.Error = "Debe especificar la contraseña actual si desea cambiar la contraseña!";
                    return sv;
                }
            }


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

            if (svm.TypeUser == "Cliente")
            {

                if (svm.AditionalAmount > 0)
                {
                    var vm = await _savingAccountService.GetPrincipalByUserId(svm.Id);
                    vm.Balance += svm.AditionalAmount;
                    await _savingAccountService.Update(vm, vm.SavingAccountId);
                }

            }
            return sv;
        }

        public async Task<SaveUserViewModel> CreateUser(SaveUserViewModel svm)
        {
            SaveUserViewModel sv = new();
            if (svm.TypeUser != "Administrador" && svm.TypeUser != "Cliente")
            {
                sv.HasError = true;
                sv.Error = "Tipo de usuario seleccionado incorrecto";
                return sv;
            }
            if (svm.TypeUser == "Cliente")
            {
                if (svm.Amount < 500 || svm.Amount > 100000 || svm.Amount == 0)
                {
                    sv.HasError = true;
                    sv.Error = "El monto debe estar entre: 500-100000";
                    return sv;
                }

            }
            var user = await _userManager.Users.ToListAsync();
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

                await _emailService.SendAsync(new EmailRequest()
                {
                    To = appliUser.Email,
                    Body = $"<h4> Saludos; </h4>" + "<p> Usted ha sido registrado en el sistema bancario BankingApp, revise su cuenta principal y haga las operaciones que tenga que hacer.</p>",
                    Subject = $"<h4>Bienvenido al sistema BankingApp {appliUser.UserName}</h4>"
                });

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
                response.Error = $"No existe una cuenta registrada con el email {request.Email}, si el problema persiste haga contacto con nuestro soporte técnico";
                return response;
            }

            var result = await _signInManager.PasswordSignInAsync(user.UserName, request.Password, false, lockoutOnFailure: false);
            if (!result.Succeeded)
            {
                response.HasError = true;
                response.Error = $"Credenciales incorrectas para el email {request.Email}, si el problema persiste haga contacto con nuestro soporte técnico";
                return response;
            }
            if (!user.EmailConfirmed)
            {
                response.HasError = true;
                response.Error = $"Su cuenta no está activa, haga contacto con nuestro soporte técnico";
                return response;
            }

            response.Id = user.Id;
            response.Email = user.Email;
            response.UserName = user.UserName;
            response.FirstName = user.FirstName;
            response.LastName = user.LastName;

            var rolesList = await _userManager.GetRolesAsync(user).ConfigureAwait(false);

            response.Roles = rolesList.ToList();
            response.IsVerified = user.EmailConfirmed;

            return response;
        }

        public async Task SignOutAsync()
        {
            await _signInManager.SignOutAsync();
        }

    }
}
