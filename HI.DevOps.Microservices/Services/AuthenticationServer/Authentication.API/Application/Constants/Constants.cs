namespace Hi.DevOps.Authentication.API.Application.Constants
{
    public static class Constants
    {
        public static class Strings
        {
            public static class JwtClaimIdentifiers
            {
                public const string Role = "rol", Id = "id", UserRole = "Role";
            }

            public static class JwtClaims
            {
                public const string ApiAccess = "api_access";
            }
        }
    }
}