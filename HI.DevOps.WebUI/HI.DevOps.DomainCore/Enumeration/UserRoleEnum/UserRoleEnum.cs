using System.ComponentModel;

namespace HI.DevOps.DomainCore.Enumeration.UserRoleEnum
{
    public enum UserRoleEnum
    {
        [Description("None")] None = 0,
        [Description("admin")] Admin = 1,
        [Description("User")] User = 2,
        [Description("Reader")] Reader = 3
    }
}