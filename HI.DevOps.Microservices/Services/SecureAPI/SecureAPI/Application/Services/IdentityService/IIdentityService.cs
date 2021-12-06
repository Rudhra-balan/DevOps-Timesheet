using Hi.DevOps.TimeSheet.API.DataObject.IdentityDO;

namespace Hi.DevOps.TimeSheet.API.Application.Services.IdentityService
{
    public interface IIdentityService
    {
        IdentityDO GetIdentity();
    }
}
