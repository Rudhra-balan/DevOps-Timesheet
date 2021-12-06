using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using log4net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;

namespace Hi.DevOps.Authentication.API.Application.Helpers
{
    public class ErrorHandlingMiddleware
    {
        private readonly IWebHostEnvironment _env;
        private readonly RequestDelegate _next;
        private readonly bool isDevelopment = false;

        public ErrorHandlingMiddleware(RequestDelegate next, IWebHostEnvironment env)
        {
            _next = next;
            _env = env;
        }


        // Mathod Invoke during request / response 

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                if (context.Response.HasStarted) throw;
                await HandleExceptionAsync(context, ex);
            }
        }


        // Handle the Error and return as Json Format

        private Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            var code = HttpStatusCode.InternalServerError; // 500 if unexpected

            if (ex is FileNotFoundException) code = HttpStatusCode.NotFound;
            else if (ex is UnauthorizedAccessException) code = HttpStatusCode.Unauthorized;
            else if (ex is InvalidOperationException) code = HttpStatusCode.NonAuthoritativeInformation;
            else if (ex != null) code = HttpStatusCode.BadRequest;


            ClearCacheHeaders(context.Response);
            ClearCookies(context);

            var result = JsonConvert.SerializeObject(new {ErrorCode = code, ErrorMessage = ex.Message});
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int) code;

            var routeData = context.GetRouteData() ?? new RouteData();

            if (!isDevelopment)
            {
                var sysLog = LogManager.GetLogger(typeof(ErrorHandlingMiddleware));
                sysLog.Debug(result);
                result = "Internal Server Error";
            }

            return context.Response.WriteAsync(result);
        }

        // Getting the Request Controller and its Paramter Value

        private static string GetRequestData(HttpContext context)
        {
            var sb = new StringBuilder();

            if (context.Request.HasFormContentType && context.Request.Form.Any())
            {
                sb.Append("Form variables:");
                foreach (var x in context.Request.Form) sb.AppendFormat("Key={0}, Value={1}<br/>", x.Key, x.Value);
            }

            sb.AppendLine("Method: " + context.Request.Method);

            return sb.ToString();
        }

        // Clearing the Cache 
        private static void ClearCacheHeaders(HttpResponse response)
        {
            response.Headers[HeaderNames.CacheControl] = "no-cache";
            response.Headers[HeaderNames.Pragma] = "no-cache";
            response.Headers[HeaderNames.Expires] = "-1";
            response.Headers.Remove(HeaderNames.ETag);
        }


        // Clearing the cookies during the error occur
        private static void ClearCookies(HttpContext context)
        {
            foreach (var cookie in context.Request.Cookies.Keys) context.Response.Cookies.Delete(cookie);
        }
    }
}