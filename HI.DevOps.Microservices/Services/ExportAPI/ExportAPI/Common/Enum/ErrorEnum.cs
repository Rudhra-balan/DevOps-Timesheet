using System.ComponentModel;

namespace Hi.DevOps.Export.API.Common.Enum
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

        [Description("An Error Occured During Export the TimeSheet.Please Try Again.")]
        DataExportTimeSheetByDateError = 501,

        [Description("An Error Occured During Export the TimeSheet.Please Try Again.")]
        DataExportTimeSheetByDateAndDepartError = 502,

        #endregion
    }
}