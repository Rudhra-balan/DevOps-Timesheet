namespace Hi.DevOps.Export.API.Application.Constants
{
    public static class RepoConstants
    {
		#region Sql Queries

        public static readonly string SQL_GET_ALL_USER = @"select EmailAddress from tblDevOpsUserInfo";


        public static readonly string SQL_EXPORT_TIMESHEET = @$"SELECT
                                                                    Username              ,
                                                                    EmailAddress          ,
                                                                    DepartmentId          ,
                                                                    Project               ,
                                                                    Epic                  ,
                                                                    Feature               ,
                                                                    UserStory             ,
                                                                    Requirements          ,
                                                                    Task                  ,
                                                                    ISNULL([1],0)                                                            AS Week1,
                                                                    ISNULL([2],0)                                                            AS Week2,
                                                                    ISNULL([3],0)                                                            AS Week3,
                                                                    ISNULL([4],0)                                                            AS Week4,
                                                                    ISNULL([5],0)                                                            AS Week5,
                                                                    (ISNULL([1],0)+ISNULL([2],0)+ ISNULL([3],0)+ISNULL([4],0)+ISNULL([5],0)) AS Total
                                                            FROM
                                                                    (
                                                                            SELECT
                                                                                    userInfo.Username                                                                                ,
                                                                                    userInfo.EmailAddress   ,
                                                                                    DepartmentId,
                                                                                    Project                                                                                          ,
                                                                                    Epic                                                                                             ,
                                                                                    Feature                                                                                          ,
                                                                                    UserStory                                                                                        ,
                                                                                    Requirements                                                                                     ,
                                                                                    Task                                                                                             ,
                                                                                    DATEDIFF(week, DATEADD(MONTH, DATEDIFF(MONTH, 0, TimeSheetDate), 0), TimeSheetDate) +1        AS [Weeks],
                                                                                    DATEADD(DAY, 2 - DATEPART(WEEKDAY, TimeSheetDate), CAST(TimeSheetDate AS DATE)) [WeekStartDate   ]      ,
                                                                                    DATEADD(DAY, 8 - DATEPART(WEEKDAY, TimeSheetDate), CAST(TimeSheetDate AS DATE)) [WeekEndDate     ]      ,
                                                                                    TimeSheetHours                                                                                AS 'Hours'
                                                                            FROM
                                                                                    [DevOpsDB].[dbo].[tblTimeSheet] timesheet
                                                                            JOIN
                                                                                    tblDevOpsUserInfo userInfo
                                                                            ON
                                                                                    userInfo.User_Unique_id = timesheet.UserId
                                                                            WHERE
                                                                                    (DATEPART(WEEK, CAST(TimeSheetDate AS DATETIME2))= DATEPART(WEEK, CAST(TimeSheetDate AS DATETIME2))) and ( {{0}} ) )p
                                                                                    Pivot (SUM(Hours) FOR Weeks IN ([1],[2], [3], [4], [5])) AS pv";

        #endregion
    }
}