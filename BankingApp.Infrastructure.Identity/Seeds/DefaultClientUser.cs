using Microsoft.AspNetCore.Identity;
using BankingApp.Core.Application.Enums;
using BankingApp.Infrastructure.Identity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BankingApp.Core.Application.Interfaces.Services;
using BankingApp.Core.Application.ViewModels.SavingAccount;

namespace BankingApp.Infrastructure.Identity.Seeds
{
    public static class DefaultClientUser
    {
        private static readonly ISavingAccountService _savingAccountService;
        public static async Task SeedAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, ISavingAccountService savingAccountService)
        {
            ApplicationUser defaultUser = new();
            defaultUser.UserName = "defaultUser";
            defaultUser.Email = "default_user@email.com";
            defaultUser.FirstName = "John";
            defaultUser.LastName = "Doe";
            defaultUser.EmailConfirmed = true;
            defaultUser.PhoneNumberConfirmed = true;

            if(userManager.Users.All(u=> u.Id != defaultUser.Id))
            {
                var user = await userManager.FindByEmailAsync(defaultUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultUser, "123Pa$$word!");
                    await userManager.AddToRoleAsync(defaultUser, Roles.Client.ToString());
                    await _savingAccountService.Add(new SaveVM_SavingAccount() { 
                        Balance = 0.00,
                        IsPrincipal = true,
                        UserId = defaultUser.Id
                    });
                }
            }
         
        }
    }
}
