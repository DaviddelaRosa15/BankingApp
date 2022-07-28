using AutoMapper;
using BankingApp.Core.Application.Dtos.Account;
using BankingApp.Core.Application.DTOs.Email;
using BankingApp.Core.Application.Interfaces.Repositories;
using BankingApp.Core.Application.Interfaces.Services;
using BankingApp.Core.Application.ViewModels.User;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BankingApp.Core.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IAccountService _accountService;
        private readonly IMapper _mapper;

        public UserService(IAccountService accountService, IMapper mapper)
        {

            _accountService = accountService;
            _mapper = mapper;
        }

        public async Task<AuthenticationResponse> LoginAsync(LoginViewModel vm)
        {
            AuthenticationRequest loginRequest = _mapper.Map<AuthenticationRequest>(vm);
            AuthenticationResponse userResponse = await _accountService.AuthenticateAsync(loginRequest);
            return userResponse;
        }
        public async Task SignOutAsync()
        {
            await _accountService.SignOutAsync();
        }
        public async Task<List<SaveUserViewModel>> GetAllUserAdminAsync()
        {
            return await _accountService.GetAllUserAdminAsync();
        }
        public async Task<SaveUserViewModel> GetUserByIdAsync(string id)
        {
            return await _accountService.GetUserByIdAsync(id);
        }
        public async Task<SaveUserViewModel> UpdateUserAsync(SaveUserViewModel svm)
        {
            return await _accountService.UpdateUserAsync(svm);
        }

        public async Task<SaveUserViewModel> CreateUser(SaveUserViewModel svm)
        {
            return await _accountService.CreateUser(svm);
        }

        public async Task DesactiveUser(string id)
        {
            await _accountService.DesactiveUser(id);
        }
        public async Task ActiveUser(string id)
        {
            await _accountService.ActiveUser(id);
        }
        public async Task<List<SaveUserViewModel>> GetAllUserClientAsync()
        {
            return await _accountService.GetAllUserClientAsync();
        }
    }
}
