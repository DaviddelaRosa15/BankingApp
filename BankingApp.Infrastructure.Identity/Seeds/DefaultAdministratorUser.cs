using Microsoft.AspNetCore.Identity;
using BankingApp.Core.Application.Enums;
using BankingApp.Infrastructure.Identity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingApp.Infrastructure.Identity.Seeds
{
    public static class DefaultAdministratorUser
    {
        public static async Task SeedAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            ApplicationUser defaultUserAdmin = new();
            defaultUserAdmin.UserName = "adminuser";
            defaultUserAdmin.Email = "adminuser@email.com";
            defaultUserAdmin.FirstName = "Juan";
            defaultUserAdmin.LastName = "Box";
            defaultUserAdmin.EmailConfirmed = true;
            defaultUserAdmin.PhoneNumberConfirmed = true;

            if(userManager.Users.All(u=> u.Id != defaultUserAdmin.Id))
            {
                var user = await userManager.FindByEmailAsync(defaultUserAdmin.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultUserAdmin, "123Pa$$word!");
                    await userManager.AddToRoleAsync(defaultUserAdmin, Roles.Administrator.ToString());
                }
            }
         
        }
    }
}
