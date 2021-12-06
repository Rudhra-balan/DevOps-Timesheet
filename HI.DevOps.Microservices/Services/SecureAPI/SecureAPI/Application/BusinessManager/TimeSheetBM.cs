using System;
using System.Collections.Generic;
using Hi.DevOps.TimeSheet.API.Application.IBusinessManager;
using Hi.DevOps.TimeSheet.API.Application.IDataBaseRepo;
using Hi.DevOps.TimeSheet.API.Common;
using Hi.DevOps.TimeSheet.API.Common.Enum;
using Hi.DevOps.TimeSheet.API.DataObject.Error;
using Hi.DevOps.TimeSheet.API.DataObject.TimeSheet;
using log4net;
using ApplicationException = Hi.DevOps.TimeSheet.API.Common.Exception.ApplicationException;

namespace Hi.DevOps.TimeSheet.API.Application.BusinessManager
{
    public class TimeSheetBM: ITimeSheetBM
    {

        #region constructor
        public TimeSheetBM(ITimeSheetRepo timeSheetRepo)
        {

            TimeSheetRepo = timeSheetRepo;
            SysLog = LogManager.GetLogger(typeof(TimeSheetBM));
         
        }
        #endregion

        #region Private variable
        private ITimeSheetRepo TimeSheetRepo { get; }
        public ILog SysLog { get; }

        #endregion

        #region Public Member

        public ErrorDO SaveWeekTimeSheet(List<WeekTimeInfoDO> timeSheetList)
        {
            try
            {
                return TimeSheetRepo.SaveWeekTimeSheet(timeSheetList);
            }
            catch (Exception ex)
            {
                if (ex is ApplicationException)
                    throw;

                throw new ApplicationException(ErrorEnum.BMSaveWeekTimeSheetError.GetDescription(),
                    ErrorEnum.BMSaveWeekTimeSheetError, ex);
            }
        }

        public WeekTimeInfoDO SaveTimeSheet(WeekTimeInfoDO timeSheet)
        {
            try
            {
                return TimeSheetRepo.SaveTimeSheet(timeSheet);
            }
            catch (Exception ex)
            {
                if (ex is ApplicationException)
                    throw;

                throw new ApplicationException(ErrorEnum.BMSaveTimeSheetError.GetDescription(),
                    ErrorEnum.BMSaveTimeSheetError, ex);
            }
        }

        public ErrorDO UpdateTimeSheet(WeekTimeInfoDO timeSheet)
        {
            try
            {
                return TimeSheetRepo.UpdateTimeSheet(timeSheet);
            }
            catch (Exception ex)
            {
                if (ex is ApplicationException)
                    throw;

                throw new ApplicationException(ErrorEnum.BMUpdateTimeSheetError.GetDescription(),
                    ErrorEnum.BMUpdateTimeSheetError, ex);
            }
        }

        public List<WeekTimeInfoDO> GetTimeInfoList(string userID)
        {
            try
            {
                return TimeSheetRepo.GetWeekTimeInfoList(userID);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ErrorEnum.BMGetTimeSheetError.GetDescription(),
                    ErrorEnum.BMGetTimeSheetError, ex);
            }
        }

        public List<WeekTimeSheetDO> GetWeekTimeByPivotDay(string userID)
        {
            try
            {
                return TimeSheetRepo.GetWeekTimeByPivotDay(userID);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ErrorEnum.BMGetTimeSheetError.GetDescription(),
                    ErrorEnum.BMGetTimeSheetError, ex);
            }
        }

        public ErrorDO DeleteTimeSheet(string task,DateTime timeSheetDateTime)
        {
            try
            {
                return TimeSheetRepo.DeleteTimeSheet(task,timeSheetDateTime);
            }
            catch (Exception ex)
            {
                if (ex is ApplicationException)
                    throw;

                throw new ApplicationException(ErrorEnum.BMDeleteTimeSheetError.GetDescription(),
                    ErrorEnum.BMDeleteTimeSheetError, ex);
            }
        }
        #endregion
    }
}
