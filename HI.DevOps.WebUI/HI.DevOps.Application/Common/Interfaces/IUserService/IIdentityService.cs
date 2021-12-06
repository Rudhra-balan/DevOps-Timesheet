using System.Threading.Tasks;
using HI.DevOps.DomainCore.Models.Response;

namespace HI.DevOps.Application.Common.Interfaces.IUserService
{
    public interface IIdentityService
    {
        Task<string> GetUserNameAsync(string userId);

        Task<(Result Result, string UserId)> CreateUserAsync(string userName, string password);

        Task<Result> DeleteUserAsync(string userId);
    }
}