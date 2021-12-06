using System;
using System.Reflection;
using HI.DevOps.Application.BussinessManagerInterface;
using HI.DevOps.Application.Common.Exceptions;
using HI.DevOps.Application.Common.Interfaces.IRequestBroker;
using HI.DevOps.DomainCore.Enumeration.ErrorEn;
using HI.DevOps.DomainCore.Extensions;
using HI.DevOps.DomainCore.Helper.Constant;
using HI.DevOps.DomainCore.Models.Login;
using HI.DevOps.DomainCore.Models.Response;
using log4net;

namespace HI.DevOps.Application.BussinessManager
{
    public class AuthenticateBm : IAuthenticateBm
    {
        #region constructor

        public AuthenticateBm(IRequestBrokerService requestBroker)
        {
            _iRequestBrokerService = requestBroker;
            _webLog = LogManager.GetLogger(typeof(AuthenticateBm));
        }

        #endregion

        #region Public Member

        public WebClientResponse AuthenticateUser(LoginUserViewModel loginUserViewModel)
        {
            if (_webLog.IsDebugEnabled)
                _webLog.Debug(string.Format(ErrorMessageConstants.LogEnteringMethodInfo,
                    $"{typeof(AuthenticateBm)}_{MethodBase.GetCurrentMethod()}"));
            WebClientResponse webClientResponse;
            try
            {
                if (_iRequestBrokerService != null)
                    webClientResponse =
                        _iRequestBrokerService.PostRequest<UserViewModel>(UrlConstant.LoginUrl, loginUserViewModel);
                else
                    webClientResponse = new WebClientResponse
                    {
                        ErrorId = (int) ErrorEnum.BmUserAuthenticateError,
                        ErrorDescription = ErrorEnum.BmUserAuthenticateError.GetDescription()
                    };
            }
            catch (Exception ex)
            {
                if (ex.Message != null && ex is AppException &&
                    ex.Message.Contains(ErrorEnum.ServiceUnavailable.GetDescription())) throw;
                throw new AppException(ErrorEnum.BmUserAuthenticateError.GetDescription(),
                    ErrorEnum.UnknownApiError, ex);
            }

            if (_webLog.IsDebugEnabled)
                _webLog.Debug(string.Format(ErrorMessageConstants.LogExitingMethodInfo,
                    $"{typeof(UserViewModel)}_{MethodBase.GetCurrentMethod()}"));

            return webClientResponse;
        }

        #endregion


        #region Private Variable

        private readonly IRequestBrokerService _iRequestBrokerService;

        private readonly ILog _webLog;

        #endregion
    }
}