using BankingApp.Core.Application.Dtos.Account;
using BankingApp.Core.Application.ViewModels.Client;
using BankingApp.Core.Application.ViewModels.User;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BankingApp.Core.Application.Interfaces.Services
{
    public interface IAccountService
    {
        Task<AuthenticationResponse> AuthenticateAsync(AuthenticationRequest request);
        Task SignOutAsync();
        Task<SaveUserViewModel> GetUserByIdAsync(string id);
        Task<SaveUserViewModel> UpdateUserAsync(SaveUserViewModel svm);
        Task<SaveUserViewModel> CreateUser(SaveUserViewModel svm);
        Task DesactiveUser(string id);
        Task ActiveUser(string id);
        Task<CountClient> CountClient();
        Task<List<SaveUserViewModel>> GetAllUserAdminAsync();
        Task<List<SaveUserViewModel>> GetAllUserClientAsync();
    }
}