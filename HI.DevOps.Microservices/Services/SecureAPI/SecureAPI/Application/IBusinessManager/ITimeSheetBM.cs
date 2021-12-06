using System;
using System.Collections.Generic;
using Hi.DevOps.TimeSheet.API.DataObject.Error;
using Hi.DevOps.TimeSheet.API.DataObject.TimeSheet;

namespace Hi.DevOps.TimeSheet.API.Application.IBusinessManager
{
    public interface ITimeSheetBM
    {
        ErrorDO SaveWeekTimeSheet(List<WeekTimeInfoDO> timeSheetList);
        WeekTimeInfoDO SaveTimeSheet(WeekTimeInfoDO timeSheet);
        ErrorDO UpdateTimeSheet(WeekTimeInfoDO timeSheet);
        List<WeekTimeInfoDO> GetTimeInfoList(string userID);
        ErrorDO DeleteTimeSheet(string task, DateTime timeSheetDateTime);

        List<WeekTimeSheetDO> GetWeekTimeByPivotDay(string userID);
    }
}
