using HI.DevOps.DomainCore.Models.Login;
using HI.DevOps.DomainCore.Models.Response;

namespace HI.DevOps.Application.BussinessManagerInterface
{
    public interface IAuthenticateBm
    {
        WebClientResponse AuthenticateUser(LoginUserViewModel loginUserViewModel);
    }
}