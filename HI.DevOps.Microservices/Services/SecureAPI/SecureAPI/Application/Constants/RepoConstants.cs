
namespace Hi.DevOps.TimeSheet.API.Application.Constants
{
    public static class RepoConstants
    {
        #region Sql Queries
        public static readonly string SQL_INSERT_WEEKTIMESHEET =
            @" Delete From tblTimeSheet where Task=@Task and TimeSheetDate = @TimeSheetDate;
              Insert Into tblTimeSheet (UserId,Project,Epic,Feature,UserStory,Requirements,Task,TimeSheetDate,TimeSheetHours) values 
             (@UserId,@Project,@Epic,@Feature,@UserStory,@Requirements,@Task,@TimeSheetDate,@TimeSheetHours)";

        public static readonly string SQL_UPDATE_WEEKTIMESHEET =
            " UPDATE tblTimeSheet SET TimeSheetHours=@TimeSheetHours WHERE Task=@Task and TimeSheetDate=@TimeSheetDate ";

        public static readonly string SQL_GET_TIMESHEET_BY_USER_ID = @" Select Project,Epic,Feature,UserStory,Requirements,Task,
                                                                        TimeSheetDate,sum(TimeSheetHours) TimeSheetHours
                                                                        From[DevOpsDB].[dbo].[tblTimeSheet]
                                                                        where UserId=@param1
                                                                        group by Project,[Epic],[Feature],[UserStory],[Requirements], Task, TimeSheetDate  ";

        public static readonly string SQL_DELETE_TIMESHEET_BY_TIMESHEET_ID = " Delete From tblTimeSheet where Task=@param1 and TimeSheetDate= @param2";

        public static readonly string SQL_GET_WEEKTIMESHEET_PIVOT_BY_DAY = @"WITH weekTimeSheetCte AS (
                                                                              SELECT top 1000 
                                                                                Project,isnull(Epic,'') Epic,isnull(Feature,'') Feature,ISNULL(UserStory,'') UserStory, ISNULL(Requirements,'') Requirements,[Task],
	                                                                            DATEADD(DAY, 1 - DATEPART(WEEKDAY, TimeSheetDate), CAST(TimeSheetDate AS DATE)) [WeekStartDate],
	                                                                            DATEADD(DAY, 7 - DATEPART(WEEKDAY, TimeSheetDate), CAST(TimeSheetDate AS DATE)) [WeekEndDate],
                                                                                SUM(TimeSheetHours) TimeSheetHours,
                                                                                ROW_NUMBER() OVER(PARTITION BY task,TimeSheetDate  ORDER BY TimeSheetDate desc) rn,
                                                                                DATEPART(DW,CAST(TimeSheetDate AS DATETIME2)) dateofweek
                                                                              FROM tblTimeSheet
                                                                              where UserId = @param1
                                                                              Group by Task,TimeSheetDate,Project,Epic,Feature,UserStory,Requirements order by TimeSheetDate desc
                                                                              
                                                                            ) 
                                                                            SELECT  Project,isnull(Epic,'') Epic,isnull(Feature,'') Feature,ISNULL(UserStory,'') UserStory, ISNULL(Requirements,'') Requirements,Task,WeekStartDate,WeekEndDate,
                                                                            isnull([1],0) Sunday, isnull([2],0) Monday, isnull([3],0) Tuesday, isnull([4],0) 
                                                                            Wednesday, isnull([5],0) Thursday, isnull([6],0) Friday, isnull([7],0) Saturday,
                                                                            (isnull([1],0)+isnull([2],0)+isnull([3],0)+isnull([4],0)+isnull([5],0)+isnull([6],0)+isnull([6],0)) as total
                                                                            FROM weekTimeSheetCte 
                                                                            PIVOT(SUM(TimeSheetHours) FOR dateofweek IN ([1],[2],[3],[4],[5],[6],[7])) p";

        #endregion
    }
}
