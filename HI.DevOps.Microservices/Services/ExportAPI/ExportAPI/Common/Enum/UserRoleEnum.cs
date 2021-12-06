using System.ComponentModel;

namespace Hi.DevOps.Export.API.Common.Enum
{
    public enum UserRoleEnum
    {
        [Description("None")] None = 0,
        [Description("Admin")] Admin = 1,
        [Description("User")] User = 2,
        [Description("Developer")] Developer = 3
    }
}