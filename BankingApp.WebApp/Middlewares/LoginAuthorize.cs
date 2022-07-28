using Microsoft.AspNetCore.Mvc.Filters;
using System.Threading.Tasks;
using BankingApp.WebApp.Controllers;
using System.Linq;

namespace BankingApp.WebApp.Middlewares
{
    public class LoginAuthorize : IAsyncActionFilter
    {
        private readonly ValidateUserSession _userSession;

        public LoginAuthorize(ValidateUserSession userSession)
        {
            _userSession = userSession;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context,ActionExecutionDelegate next)
        {
            var user = _userSession.HasUser();
            if ( user != null)
            {
                var controller = (UserController)context.Controller;
                if (user.Roles.Any(n => n == "Administrator"))
                {
                    context.Result = controller.RedirectToAction("index", "Admin");
                }
                else
                {
                    context.Result = controller.RedirectToAction("index", "Client");
                }                
            }
            else
            {
                await next();
            }
        }
    }
}
