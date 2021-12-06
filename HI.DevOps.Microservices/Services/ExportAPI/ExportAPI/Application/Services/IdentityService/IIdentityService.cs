using Hi.DevOps.Export.API.DataObject.IdentityDO;

namespace Hi.DevOps.Export.API.Application.Services.IdentityService
{
    public interface IIdentityService
    {
        IdentityDO GetIdentity();
    }
}