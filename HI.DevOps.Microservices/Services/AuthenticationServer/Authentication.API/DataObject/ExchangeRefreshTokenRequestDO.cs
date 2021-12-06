using Hi.DevOps.Authentication.API.Common.Enum;

namespace Hi.DevOps.Authentication.API.DataObject
{
    public class ExchangeRefreshTokenRequestDo
    {
        public string UserId { get; set; }
        public string Username { get; set; }

        public UserRoleEnum UserRole { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}