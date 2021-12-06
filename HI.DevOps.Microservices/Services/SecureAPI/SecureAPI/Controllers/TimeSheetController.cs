using System;
using System.Collections.Generic;
using System.Web;
using Hi.DevOps.TimeSheet.API.Application.IBusinessManager;
using Hi.DevOps.TimeSheet.API.Common;
using Hi.DevOps.TimeSheet.API.Common.Enum;
using Hi.DevOps.TimeSheet.API.DataObject.Error;
using Hi.DevOps.TimeSheet.API.DataObject.TimeSheet;
using Microsoft.AspNetCore.Mvc;

namespace Hi.DevOps.TimeSheet.API.Controllers
{
    public class TimeSheetController : BaseController
    {
        #region Private Variable

        private readonly ITimeSheetBM _iTimeSheetBM;

        #endregion

        #region Constructor

        public TimeSheetController(ITimeSheetBM iTimeSheetBM)
        {
            _iTimeSheetBM = iTimeSheetBM;
        }

        #endregion

        #region Public Member

        [HttpPost("SaveWeekTimeSheet")]
        public ErrorDO SaveWeekTimeSheet([FromBody] List<WeekTimeInfoDO> timeSheetList)
        {
            try
            {
                return _iTimeSheetBM.SaveWeekTimeSheet(timeSheetList);
            }
            catch (Exception ex)
            {
                switch (ex)
                {
                    case ApplicationException _:
                    {
                        return new ErrorDO
                        {
                            Id = (int) ex.Data["ErrorId"],
                            Message = (string) ex.Data["ErrorDesc"]
                        };
                    }
                    default:
                        return new ErrorDO
                        {
                            Id = (int) ErrorEnum.UnknownApiError,
                            Message = ErrorEnum.UnknownApiError.GetDescription()
                        };
                }
            }
        }

        [HttpPost("SaveTimeSheet")]
        public WeekTimeInfoDO SaveTimeSheet([FromBody] WeekTimeInfoDO timeSheet)
        {
            try
            {
                return _iTimeSheetBM.SaveTimeSheet(timeSheet);
            }
            catch (Exception ex)
            {
                switch (ex)
                {
                    case ApplicationException _:
                    {
                        timeSheet.ErrorListDo.Add( new ErrorDO
                        {
                            Id = (int) ex.Data["ErrorId"],
                            Message = (string) ex.Data["ErrorDesc"]
                        });
                        break;
                    }
                    default:
                        timeSheet.ErrorListDo.Add(new ErrorDO
                        {
                            Id = (int) ErrorEnum.UnknownApiError,
                            Message = ErrorEnum.UnknownApiError.GetDescription()
                        });
                        break;
                }
            }

            return timeSheet;
        }

        [HttpPost("UpdateTimeSheet")]
        public ErrorDO UpdateTimeSheet([FromBody] WeekTimeInfoDO timeSheet)
        {
            try
            {
                return _iTimeSheetBM.UpdateTimeSheet(timeSheet);
            }
            catch (Exception ex)
            {
                switch (ex)
                {
                    case ApplicationException _:
                    {
                        return new ErrorDO
                        {
                            Id = (int)ex.Data["ErrorId"],
                            Message = (string)ex.Data["ErrorDesc"]
                        };
                    }
                    default:
                        return new ErrorDO
                        {
                            Id = (int)ErrorEnum.UnknownApiError,
                            Message = ErrorEnum.UnknownApiError.GetDescription()
                        };
                }
            }
        }

        [HttpPost("GetTimeInfoList/{UserId}")]
        public List<WeekTimeInfoDO> GetTimeInfoList(string userId)
        {
            return _iTimeSheetBM.GetTimeInfoList(HttpUtility.UrlDecode(userId));
        }

        [HttpPost("GetWeekTimeByPivotDay/{UserId}")]
        public List<WeekTimeSheetDO> GetWeekTimeByPivotDay(string userId)
        {
            return _iTimeSheetBM.GetWeekTimeByPivotDay(HttpUtility.UrlDecode(userId));
        }

        [HttpPost("deleteTimeSheet/{date}/{task}")]
        public ErrorDO DeleteTimeSheet(string date, string task)
        {
            try
            {
                return _iTimeSheetBM.DeleteTimeSheet(task, Convert.ToDateTime(date));
            }
            catch (Exception ex)
            {
                switch (ex)
                {
                    case ApplicationException _:
                    {
                        return new ErrorDO
                        {
                            Id = (int)ex.Data["ErrorId"],
                            Message = (string)ex.Data["ErrorDesc"]
                        };
                    }
                    default:
                        return new ErrorDO
                        {
                            Id = (int)ErrorEnum.UnknownApiError,
                            Message = ErrorEnum.UnknownApiError.GetDescription()
                        };
                }
            }
        }
        

        #endregion
    }
}