using BankingApp.Core.Application.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using BankingApp.Core.Application.Interfaces.Services;
using System.Reflection;

namespace BankingApp.Core.Application
{

    //Extension Method - Decorator
    public static class ServiceRegistration
    {
        public static void AddApplicationLayer(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            #region Services
            services.AddTransient(typeof(IGenericService<,,>), typeof(GenericService<,,>));
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IProductClient, ProductClientServices>();
            services.AddTransient<ISavingAccountService, SavingAccountService>();
            services.AddTransient<IBeneficiaryService, BeneficiaryService>();
            services.AddTransient<ILoanService, LoanService>();
            services.AddTransient<ICreditCardService, CreditCardService>();
            services.AddTransient<ITransactionService, TransactionService>();
            services.AddTransient<IOperationService, OperationService>();
            #endregion
        }
    }
}
