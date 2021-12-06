using Hi.DevOps.Authentication.API.DataObject;

namespace Hi.DevOps.Authentication.API.Application.DatabaseRepoInterface
{
    public interface IJwtFactoryRepo
    {
       
        bool UpdateRefreshToken(string token);

        User GetUserInformation(string emailAddress);

    }
}