namespace HI.DevOps.DomainCore.Helper.Constant
{
    public class UrlConstant
    {
        #region Web Api Url

        public const string LoginUrl = "/HI.DevOps.Security.Api/AuthenticateUser";

        #endregion

        #region Controller Route

        public const string Authorize = "Authorize";
        public const string WeekView = "WeekView";
        public const string MonthView = "MonthView";
        public const string AddWeekView = "AddWeekView/{id}";
        public const string AddQueryControl = "AddQueryControl/{id}";
        public const string DeleteTimeSheet = "DeleteTimeSheet/{date}/{task}";
        public const string GetTaskList = "GetTaskList/{projectName}";
        public const string SaveWeeKTimeWorkItems = "SaveWeeKTimeWorkItems";
        public const string SaveTimeWorkItems = "SaveTimeWorkItems";
        public const string UpdateTimeWorkItems = "UpdateTimeWorkItems";
        public const string GetCalenderData = "GetCalenderData";
        public const string ExportView = "Export";
        public const string ExportTimeSheet = "ExportTimeSheet";
        public const string NextPrevious = "Nextprevious/{startDate}/{endDate}";
        public const string WeekListByDate = "WeekListByDate/{startDate}/{endDate}";
      

        #endregion

        #region View Constant

        public const string TimeSheetWeekViewCshtml = "~/Views/TimeSheet/TimeSheetWeekEntry.cshtml";
        public const string TimeSheetMonthViewCshtml = "~/Views/TimeSheet/TimeSheetMonthEntry.cshtml";
        public const string ExportViewCshtml = "~/Views/Export/Export.cshtml";
        public const string QueryViewCshtml = "~/Views/Export/QueryControl.cshtml";
        public const string DashBoardViewCshtml = "~/Views/DashBoard/DashBoard.cshtml";
        public const string AuthenticationViewCshtml = "~/Views/Authentication/Index.cshtml";

        #endregion
    }
}