using System.Threading.Tasks;

namespace HI.DevOps.Application.Common.Interfaces.IUserService
{
    public interface IIdentityServerClient
    {
        public Task<string> RequestClientCredentialsTokenAsync();
    }
}