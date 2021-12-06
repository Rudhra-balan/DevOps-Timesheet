using System.ComponentModel;

namespace HI.DevOps.DomainCore.Enumeration.ErrorEn
{
    public enum ErrorEnum
    {
        #region Web-Errors

        [Description("Unknown Web Client error has occurred")]
        UnknownWebClientError = 699,

        #endregion

        #region API-Errors

        [Description("Post request parameters, url and/or dataObject is null. <br/> Please check parameters.")]
        PostRequestParametersNull = 101,

        [Description("Value is out of range")] OutOfRangeError = 102,

        [Description("File not found.")] FileNotFoundError = 103,

        [Description("Unknown API  error has occurred")]
        UnknownApiError = 104,

        [Description("Service Temporarily Unavailable")]
        ServiceUnavailable = 106,

        [Description("")] NotDefinedError = 999,

        #endregion

        #region Bussiness Error

        [Description("Error in User Authenticate.Please Try again. ")]
        BmUserAuthenticateError = 101,

        [Description("Unauthorized user")] UserAuthenticateError = 102,

        [Description("Invalid Username or Password")]
        InvalidUserNameOrPassWord = 103,

        #endregion
    }
}