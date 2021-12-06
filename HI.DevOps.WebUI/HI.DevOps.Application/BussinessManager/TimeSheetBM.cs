using System;
using System.Reflection;
using HI.DevOps.Application.BussinessManagerInterface;
using HI.DevOps.Application.Common.Exceptions;
using HI.DevOps.Application.Common.Interfaces.IRequestBroker;
using HI.DevOps.DomainCore.Enumeration.ErrorEn;
using HI.DevOps.DomainCore.Extensions;
using HI.DevOps.DomainCore.Helper.Constant;
using HI.DevOps.DomainCore.Models.TimeSheet;
using log4net;

namespace HI.DevOps.Application.BussinessManager
{
    public class TimeSheetBM : ITimeSheetBM
    {
        #region Constructor

        public TimeSheetBM(IRequestBrokerService requestBroker)
        {
            _iRequestBrokerService = requestBroker;
            _webLog = LogManager.GetLogger(typeof(TimeSheetBM));
        }

        #endregion

        #region Public Member

        public TimeSheetViewModel Initialize(TimeSheetViewModel timeSheetViewModel)
        {
            if (_webLog.IsDebugEnabled)
                _webLog.Debug(string.Format(ErrorMessageConstants.LogEnteringMethodInfo,
                    $"{typeof(TimeSheetBM)}_{MethodBase.GetCurrentMethod()}"));

            try
            {
                if (timeSheetViewModel == null)
                    timeSheetViewModel = new TimeSheetViewModel();

                
            }
            catch (Exception ex)
            {
                if (ex.Message != null && ex is AppException &&
                    ex.Message.Contains(ErrorEnum.UnknownApiError.GetDescription())) throw;
                throw new AppException(ErrorEnum.UnknownApiError.GetDescription(),
                    ErrorEnum.UnknownApiError, ex);
            }

            if (_webLog.IsDebugEnabled)
                _webLog.Debug(string.Format(ErrorMessageConstants.LogExitingMethodInfo,
                    $"{typeof(TimeSheetBM)}_{MethodBase.GetCurrentMethod()}"));

            return timeSheetViewModel;
        }

        #endregion

        #region Private Variable

        private readonly IRequestBrokerService _iRequestBrokerService;

        private readonly ILog _webLog;

        #endregion
    }
}