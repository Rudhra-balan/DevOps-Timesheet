using System;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using HI.DevOps.Application.Common.Exceptions;
using HI.DevOps.Application.Common.Interfaces.IRequestBroker;
using HI.DevOps.DomainCore.Enumeration.ErrorEn;
using HI.DevOps.DomainCore.Extensions;
using HI.DevOps.DomainCore.Helper;
using HI.DevOps.DomainCore.Helper.Constant;
using HI.DevOps.DomainCore.Models.Response;
using Newtonsoft.Json;

namespace HI.DevOps.Infrastructure.Helper

{
    public class RequestBroker : IRequestBrokerService
    {
        #region Private Methods

        /// <summary>
        ///     Posts the url to invoke the API Services
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="dataObject"></param>
        /// <returns>
        ///     <see cref="WebClientResponse" />
        /// </returns>
        private WebClientResponse PostRequestAction<T>(string url, object dataObject)
        {
            var webClientResponse = new WebClientResponse();
            try
            {
                if (string.IsNullOrEmpty(url))
                {
                    webClientResponse.ErrorId = (int) ErrorEnum.PostRequestParametersNull;
                    webClientResponse.ErrorDescription = ErrorEnum.PostRequestParametersNull.GetDescription();
                    webClientResponse.IsOperationSuccess = false;

                    return webClientResponse;
                }

                var timeoutMinutes = ConfigReader.QuickRead("WebClientTimeout").ToInt();

                var timeoutSpan = timeoutMinutes > 0
                    ? TimeSpan.FromMilliseconds(timeoutMinutes)
                    : Timeout.InfiniteTimeSpan;

                using (var client = new HttpClient())
                {
                    var stringContent = new StringContent(string.Empty);
                    client.Timeout = timeoutSpan;
                    if (dataObject != null)
                    {
                        var dataSerialized = JsonConvert.SerializeObject(dataObject);
                        stringContent = new StringContent(dataSerialized, Encoding.UTF8, "application/json");
                    }

                    var serviceUrl = _baseApiUrl + url;

                    var result = client.PostAsync(serviceUrl, stringContent).Result;
                    if (!result.IsSuccessStatusCode)
                        return default;
                    var data = result.Content.ReadAsStringAsync().Result;


                    T sourceObject;

                    if (typeof(T) == typeof(bool)) sourceObject = (T) Convert.ChangeType(data, typeof(bool));
                    else if (typeof(T) == typeof(string)) sourceObject = (T) Convert.ChangeType(data, typeof(string));
                    else sourceObject = JsonConvert.DeserializeObject<T>(data);


                    webClientResponse.ErrorId = (int) result.StatusCode;
                    webClientResponse.SourceObject = sourceObject;

                    if (result.StatusCode == HttpStatusCode.OK
                        || result.StatusCode == HttpStatusCode.NoContent)
                    {
                        webClientResponse.IsOperationSuccess = true;
                        webClientResponse.ErrorDescription = result.ReasonPhrase;
                    }
                    else
                    {
                        webClientResponse.IsOperationSuccess = false;
                        var errorTypeEnum = (ErrorTypeEnum) webClientResponse.ErrorId;
                        webClientResponse.ErrorDescription = errorTypeEnum.GetDescription();
                    }
                }


                return webClientResponse;
            }

            catch (Exception ex)
            {
                switch (ex)
                {
                    case SocketException _:
                    case HttpRequestException _:
                    case AggregateException _:
                        webClientResponse.IsOperationSuccess = false;
                        webClientResponse.ErrorId = ErrorEnum.ServiceUnavailable.ToInt();
                        webClientResponse.ErrorDescription = ErrorEnum.ServiceUnavailable.GetDescription();
                        break;
                    default:
                        webClientResponse.IsOperationSuccess = false;
                        webClientResponse.ErrorId = ErrorEnum.UnknownApiError.ToInt();
                        webClientResponse.ErrorDescription = ErrorEnum.UnknownApiError.GetDescription();
                        break;
                }
            }

            return webClientResponse;
        }

        #endregion

        #region Constructor

        /// <summary>
        ///     Returns the Base portion of the url for API Services
        /// </summary>
        private readonly string _baseApiUrl;

        public RequestBroker(string baseApiUrl)
        {
            _baseApiUrl = baseApiUrl;
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Represents the ErrorId returned by the API Services
        /// </summary>
        public static int ErrorId { get; set; }


        /// <summary>
        ///     SendRequest invokes the webClient's downloadString method
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="requiresAuthentication"></param>
        /// <returns>
        ///     <see cref="T" />
        /// </returns>
        public T SendRequest<T>(string url, bool requiresAuthentication = false)
        {
            try
            {
                var timeoutMinutes = ConfigReader.QuickRead("WebClientTimeout").ToInt();

                T result;
                using (var client = new WebClientEx(timeoutMinutes))
                {
                    var serviceUrl = _baseApiUrl + url;

                    client.Encoding = Encoding.UTF8;

                    var downloadTask = client.DownloadStringTaskAsync(new Uri(serviceUrl));

                    var data = downloadTask.Result;

                    if (downloadTask.IsFaulted)
                        return default;

                    if (typeof(T) == typeof(bool)) result = (T) Convert.ChangeType(data, typeof(bool));
                    else if (typeof(T) == typeof(string)) result = (T) Convert.ChangeType(data, typeof(string));
                    else result = JsonConvert.DeserializeObject<T>(data);
                }


                return result;
            }
            catch (AggregateException ae)
            {
                ae.Handle(exception =>
                {
                    if (exception is SocketException || exception is HttpRequestException)
                        throw new AppException(string.Format(AppConstants.GenericApiActionError,
                            ErrorEnum.ServiceUnavailable.GetDescription(), ErrorEnum.ServiceUnavailable.ToInt()));

                    throw new AppException(string.Format(AppConstants.GenericApiActionError,
                        ErrorEnum.UnknownApiError.GetDescription(), ErrorEnum.UnknownApiError.ToInt()));
                });

                throw new AppException(string.Format(AppConstants.GenericApiActionError,
                    ErrorEnum.UnknownApiError.GetDescription(), ErrorEnum.UnknownApiError.ToInt()));
            }
            catch (Exception ex)
            {
                if (ex is SocketException || ex is HttpRequestException)
                    throw new AppException(string.Format(AppConstants.GenericApiActionError,
                        ErrorEnum.ServiceUnavailable.GetDescription(), ErrorEnum.ServiceUnavailable.ToInt()));

                throw new AppException(string.Format(AppConstants.GenericApiActionError,
                    ErrorEnum.UnknownApiError.GetDescription(), ErrorEnum.UnknownApiError.ToInt()));
            }
        }

        /// <summary>
        ///     This is the overloaded method invoking the PostSync method of the API services
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="dataObject"></param>
        /// <returns></returns>
        public WebClientResponse PostRequest<T>(string url, object dataObject)
        {
            var webClientResponse = new WebClientResponse();
            if (dataObject == null)
            {
                webClientResponse.ErrorId = (int) ErrorEnum.PostRequestParametersNull;
                webClientResponse.ErrorDescription = ErrorEnum.PostRequestParametersNull.GetDescription();
                webClientResponse.IsOperationSuccess = false;

                return webClientResponse;
            }

            return PostRequestAction<T>(url, dataObject);
        }

        /// <summary>
        ///     This is the overloaded method invoking the PostSync method of the API services
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <returns></returns>
        public WebClientResponse PostRequest<T>(string url)
        {
            return PostRequestAction<T>(url, null);
        }

        #endregion
    }
}