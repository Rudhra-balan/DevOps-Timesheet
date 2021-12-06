using System.Threading.Tasks;
using Hi.DevOps.Authentication.API.DataObject;

namespace Hi.DevOps.Authentication.API.Application.BussinessManagerInterface
{
    public interface IJwtFactoryBm
    {
        Task<UserResponseDo> AuthenticateAsync(string username, string devOpsUserDescriptor);

        Task<ExchangeRefreshTokenResponseDo> RefreshTokenAsync(
            ExchangeRefreshTokenRequestDo exchangeRefreshTokenRequestDo);
    }
}