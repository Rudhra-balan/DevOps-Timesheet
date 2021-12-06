namespace HI.DevOps.DomainCore.Helper.Constant
{
    public class AppConstants
    {
        #region Error Constant

        public const string IdInvalidError = "Id {0} is invalid. <br/>  Please contact your system administrator";
        public const string GenericApiActionError = "{0} <br/><br/> Error Code: {1}";

        public const string GenericApiError =
            "{0} - Unable to perform API Services. <br/> Please contact your system administrator";

        public const string GenericWebError =
            "A critical error has occured: {0} - Unable to finish the operation successfully. <br/>  Please contact your system administrator";

        public const string GenericDmError =
            "A critical error has occured in the device manager. <br/> Please contact your system administrator";

        #endregion

        #region SESSION KEYS

        public const int SessionIdleSystemTimeout = 1440; // 24hrs
        public const int SessionIdleDefaultTimeout = 30;
        public const int SessionIdleMaxTimeout = 60;
        public const int SessionIdleMinTimeout = 0;
        public const string SessionCaptchaString = "SESSION_CAPTCHA_STRING";
        public const string SessionAccessKey = "SESSION_ACCESS_KEY";
        public const string SessionRefreshKey = "SESSION_REFRESH_KEY";
        public const string SessionAccessKeyTimeOut = "SESSION_ACCESS_KEY_TIMEOUT";
        public const string CacheSecuritySettings = "CACHE_SECURITY_SETTINGS";
        public const string SessionUserName = "SESSION_USER_NAME";
        public const string SessionUserData = "SESSION_USER_DATA";
        public const string SessionLogoutFlag = "SESSION_LOGOUT_FLAG";
        public const string SessionIdleKey = "IdleTimeout";
        public const string DevOpsAccessKey = "DEVOPS_ACCESS_KEY";
        public const string DevOpsRefreshKey = "DEVOPS_REFRESH_KEY";
        public const string DevOpsAccessKeyTimeOut = "DEVOPS_ACCESS_KEY_TIMEOUT";
        public const string SessionUser = "SESSION_USER";
        public const string SessionUserID = "SESSION_USER_ID";

        #endregion

        #region Cache Constant

        public const string DevOpsClientInfo = "devOpsClientInfo";
        public const string TokenInfo = "tokenInfo";
        public const string DevOpsUser = "devOpsUser";
        public const string WorkItemList = "workItemList";
        public const string UserList = "userList";
        public const string TimeSheetList = "TimeSheetList";
        public const string WeekTimeSheetList = "weektimeSheetList";

        #endregion
    }
}