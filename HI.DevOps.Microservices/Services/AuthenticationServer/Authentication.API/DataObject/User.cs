using Hi.DevOps.Authentication.API.Common.Enum;

namespace Hi.DevOps.Authentication.API.DataObject
{
    public class User : BaseDo
    {
        public string UserIdentityId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public UserRoleEnum UserRole { get; set; }
        public bool IsActive { get; set; }
    }
}