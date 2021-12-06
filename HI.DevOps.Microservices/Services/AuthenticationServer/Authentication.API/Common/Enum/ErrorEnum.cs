using System.ComponentModel;

namespace Hi.DevOps.Authentication.API.Common.Enum
{
    public enum ErrorEnum
    {
        #region API Error

        [Description("Post request parameters, url and/or dataObject is null. <br/> Please check parameters.")]
        PostRequestParametersNull = 100,

        [Description("Invalid Refresh Token")] InvalidRefreshToken = 101,

        [Description("Unauthorized user")] UserAuthenticateError = 102,

        [Description("User Registration Error")]
        UserRegistrationError = 103,


        [Description("Invalid Username or Password")]
        InvalidUserNameOrPassWord = 104,

        [Description("Unknown API Services error has occurred")]
        UnknownApiError = 999,

        #endregion

        #region DataBase Error

        [Description("Error in Database User Authenticate ")]
        DatabaseUserAuthenticateError = 502,

        [Description("Error in Database Getting User identity id ")]
        DatabaseUserGettingUserIdError = 503,


        [Description("Error in Database Update RefreshToken")]
        DatabaseUpdateRefreshToken = 504,

        [Description("Error in Database User Registeration ")]
        DatabaseUserRegisterationError = 505,

        #endregion
    }
}