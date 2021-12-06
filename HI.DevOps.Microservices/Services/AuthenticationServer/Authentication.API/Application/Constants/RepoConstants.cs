namespace Hi.DevOps.Authentication.API.Application.Constants
{
    public class RepoConstants
    {
        public static string SqlAuthenticateUser = "Sp_Authenticate_User";

        public static string SqlGenerateRefreshToken = "SP_Generate_RefreshToken";

        public static readonly string SqlGetUser = "Select User_Unique_id,EmailAddress,UserRole,IsActive from tblDevOpsUserInfo where EmailAddress = @param1; ";
        
        public static string SqlRegisterUser = "Sp_UserRegistration";
    }
}