using System.ComponentModel;

namespace Hi.DevOps.TimeSheet.API.Common.Enum
{
    public enum ErrorEnum
    {
        #region API Error

        [Description("Post request parameters, url and/or dataObject is null. <br/> Please check parameters.")]
        PostRequestParametersNull = 100,

        

        [Description("Unauthorized user")] UserAuthenticateError = 102,
        [Description("An Error Occured During Saving the TimeSheet.Please Try Again.")]
        BMSaveWeekTimeSheetError = 103,

        [Description("An Error Occured During Saving the TimeSheet.Please Try Again.")]
        BMSaveTimeSheetError = 104,
        [Description("An Error Occured During Update the TimeSheet.Please Try Again.")]
        BMUpdateTimeSheetError = 105,
        [Description("An Error Occured During Getting the TimeSheet.Please Try Again.")]
        BMGetTimeSheetError = 106,
        [Description("An Error Occured During Deleteing the TimeSheet.Please Try Again.")]
        BMDeleteTimeSheetError = 107,

        [Description("Unknown API Services error has occurred")]
        UnknownApiError = 999,

        #endregion

        #region DataBase Error

        [Description("An Error Occured During Saving the TimeSheet.Please Try Again.")]
        DatabaseSaveWeekTimeSheetError = 502,

        [Description("An Error Occured During Saving the TimeSheet.Please Try Again.")]
        DatabaseSaveTimeSheetError = 503,
        [Description("An Error Occured During update the TimeSheet.Please Try Again.")]
        DatabaseUpdateTimeSheetError = 504,
        [Description("An Error Occured During Getting the TimeSheet.Please Try Again.")]
        DataGetTimeSheetError = 506,
        [Description("An Error Occured During Delete the TimeSheet.Please Try Again.")]
        DataDeleteTimeSheetError = 507,


        #endregion
    }
}