using System;
using HI.DevOps.DomainCore.Enumeration.ErrorEn;
using HI.DevOps.DomainCore.Extensions;
using log4net;

namespace HI.DevOps.Application.Common.Exceptions
{
    public class AppException : Exception
    {
        //private ErrorTypeEnum _errorType = ErrorTypeEnum.NotDefined;

        private static ILog _serviceLog;

        /// <summary>
        ///     Default constructor
        /// </summary>
        public AppException()
        {
            _serviceLog = LogManager.GetLogger(typeof(AppException));
        }

        /// <summary>
        ///     This constructor will get the message as parameter
        ///     and it will be used when
        ///     this exception class needs to be instantiated with
        ///     exception message
        /// </summary>
        /// <param name="message">exception message</param>
        public AppException(string message) : base(message)
        {
            LogException(message, ErrorEnum.UnknownApiError.ToInt(), ErrorEnum.UnknownApiError.GetDescription());
        }

        /// <summary>
        ///     This constructor will get the message and innerException
        ///     exception as parameter and it will
        ///     be used when this exception class needs to be
        ///     instantiated with exception message
        ///     and inner exception
        /// </summary>
        /// <param name="errorSource">exception message source</param>
        /// <param name="errorType">
        ///     the error type - this will be added as
        ///     Data in the exception's Data member
        /// </param>
        public AppException(string errorSource, ErrorEnum errorType) : base(
            errorSource + " - " + errorType.GetDescription())
        {
            LogException(errorSource, errorType.ToInt(), errorType.GetDescription());
        }

        /// <summary>
        ///     This constructor will get the message and innerException
        ///     exception as parameter and it will
        ///     be used when this exception class needs to be
        ///     instantiated with exception message
        ///     and inner exception
        /// </summary>
        /// <param name="message">exception message</param>
        /// <param name="errorType">
        ///     the error type - this will be added as
        ///     <param name="exception">The exception that caused this exception</param>
        ///     Data in the exception's Data member
        /// </param>
        /// <param name="exception"></param>
        public AppException(string message, ErrorEnum errorType, Exception exception) : base(message, exception)
        {
            LogException(message, errorType.ToInt(), errorType.GetDescription(), exception);
        }

        /// <summary>
        ///     This constructor will get the message and innerException
        ///     exception as parameter and it will
        ///     be used when this exception class needs to be
        ///     instantiated with exception message
        ///     and inner exception
        /// </summary>
        /// <param name="message">exception message</param>
        /// <param name="exception"></param>
        public AppException(string message, Exception exception) : base(message, exception)
        {
            LogException(message, ErrorEnum.UnknownApiError.ToInt(), ErrorEnum.UnknownApiError.GetDescription(),
                exception);
        }


        #region Private Methods

        private void LogException(string errorSource, int errorId, string errorDescription, Exception exception = null)
        {
            var errorFormat = $"{errorSource} -- Error Id: {errorId}, {errorDescription} -- ";


            Data.Add("ErrorId", errorId);
            Data.Add("ErrorDesc", errorDescription);
            Data.Add("ErrorSource", errorSource);

            _serviceLog = LogManager.GetLogger(typeof(AppException));

            if (_serviceLog.IsErrorEnabled)
                _serviceLog.Error(errorFormat, exception);
        }

        #endregion
    }
}