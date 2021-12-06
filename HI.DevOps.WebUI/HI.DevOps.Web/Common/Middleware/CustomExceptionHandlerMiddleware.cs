using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using log4net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Hosting;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;

namespace HI.DevOps.Web.Common.Middleware
{
    public class CustomExceptionHandlerMiddleware
    {
        private readonly bool _isDevelopment;
        private readonly RequestDelegate _next;

        public CustomExceptionHandlerMiddleware(RequestDelegate next, IWebHostEnvironment env)
        {
            _next = next;
            _isDevelopment = env.IsDevelopment();
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var code = HttpStatusCode.InternalServerError; // 500 if unexpected
            var result = string.Empty;

            switch (exception)
            {
                case FileNotFoundException _:
                    code = HttpStatusCode.NotFound;
                    break;
                case UnauthorizedAccessException _:
                    code = HttpStatusCode.Unauthorized;
                    break;
                case InvalidOperationException _:
                    code = HttpStatusCode.NonAuthoritativeInformation;
                    break;
                case { } _:
                    code = HttpStatusCode.BadRequest;
                    break;
            }


            ClearCacheHeaders(context.Response);
            ClearCookies(context);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int) code;
            var routeData = context.GetRouteData() ?? new RouteData();

            if (string.IsNullOrEmpty(result)) result = JsonConvert.SerializeObject(new {error = exception.Message});
            if (_isDevelopment) return context.Response.WriteAsync(result);
            var sysLog = LogManager.GetLogger(typeof(CustomExceptionHandlerMiddleware));
            sysLog.Debug(result);
            result = "Internal Server Error";

            return context.Response.WriteAsync(result);
        }

        // Clearing the Cache 
        private static void ClearCacheHeaders(HttpResponse response)
        {
            response.Headers[HeaderNames.CacheControl] = "no-cache";
            response.Headers[HeaderNames.Pragma] = "no-cache";
            response.Headers[HeaderNames.Expires] = "-1";
            response.Headers.Remove(HeaderNames.ETag);
        }


        private static void ClearCookies(HttpContext context)
        {
            foreach (var cookie in context.Request.Cookies.Keys) context.Response.Cookies.Delete(cookie);
        }
    }


    public static class CustomExceptionHandlerMiddlewareExtensions
    {
        public static IApplicationBuilder UseCustomExceptionHandler(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CustomExceptionHandlerMiddleware>();
        }
    }
}