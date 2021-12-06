using Hi.DevOps.Export.API.Common.Enum;
using log4net;

namespace Hi.DevOps.Export.API.Common.Exception
{
    public class ApplicationException : System.Exception
    {
        /// <summary>
        ///     Default constructor
        /// </summary>
        public ApplicationException()
        {
            ServiceLog = LogManager.GetLogger(typeof(ApplicationException));
        }


        public ApplicationException(string message) : base(message)
        {
            LogException(message, ErrorEnum.UnknownApiError.ToInt(), ErrorEnum.UnknownApiError.GetDescription());
        }

        public ApplicationException(string message, System.Exception ex = null) : base(message, ex)
        {
            LogException(message, ErrorEnum.UnknownApiError.ToInt(), ErrorEnum.UnknownApiError.GetDescription(), ex);
        }

        public ApplicationException(string message, ErrorEnum errorEnum, System.Exception ex = null) : base(message, ex)
        {
            LogException(message, errorEnum.ToInt(), errorEnum.GetDescription(), ex);
        }

        public static ILog ServiceLog { get; private set; }

        private void LogException(string errorSource, int errorId, string errorDescription,
            System.Exception exception = null)
        {
            var errorFormat = "{0} -- Error Id: {1}, {2} -- ";
            var formattedErrorMessage = string.Format(errorFormat, errorSource, errorId, errorDescription);

            Data.Add("ErrorId", errorId);
            Data.Add("ErrorDesc", errorDescription);
            Data.Add("ErrorSource", errorSource);

            ServiceLog = LogManager.GetLogger(typeof(ApplicationException));

            if (ServiceLog.IsErrorEnabled)
                ServiceLog.Error(formattedErrorMessage, exception);
        }
    }
}