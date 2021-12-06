using System;
using System.Net;
using HI.DevOps.DomainCore.Extensions;
using log4net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc.Filters;

namespace HI.DevOps.Application.Common.Exceptions
{
    public class CustomExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            var sysLog = LogManager.GetLogger(typeof(CustomExceptionFilter));

            if (sysLog.IsErrorEnabled)
                if (context != null)
                    sysLog.Error(context.Exception);

            int status;
            string message;

            var exceptionType = context?.Exception.GetType();
            if (exceptionType == typeof(UnauthorizedAccessException))
            {
                message = "Unauthorized Access";
                status = HttpStatusCode.Unauthorized.ToInt();
            }
            else if (exceptionType == typeof(NotImplementedException))
            {
                message = "Server error (nie)";
                status = HttpStatusCode.InternalServerError.ToInt();
            }
            else if (exceptionType == typeof(AppException))
            {
                message = context.Exception.Message;
                status = Convert.ToInt32(context.Exception.Data["ErrorId"]);
            }
            else if (exceptionType == typeof(NullReferenceException))
            {
                message = "Server error (nre)";
                status = HttpStatusCode.InternalServerError.ToInt();
            }
            else
            {
                message = context?.Exception.Message;
                status = HttpStatusCode.InternalServerError.ToInt();
            }

            if (context != null)
            {
                context.ExceptionHandled = true;
                var response = context.HttpContext.Response;
                response.StatusCode = status;
                response.HttpContext.Features.Get<IHttpResponseFeature>().ReasonPhrase = message;
                response.ContentType = "application/json";

                // display stack trace if in development but always record it in log.
                var err = context.Exception.StackTrace;
                sysLog.Debug(err);
                response.WriteAsync(err);
            }
        }
    }
}