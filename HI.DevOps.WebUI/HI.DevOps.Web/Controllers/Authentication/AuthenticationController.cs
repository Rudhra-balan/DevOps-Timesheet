using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using HI.DevOps.Application.BussinessManager;
using HI.DevOps.Application.BussinessManagerInterface;
using HI.DevOps.Application.Common.Exceptions;
using HI.DevOps.Application.Common.Interfaces.IDevOpsRequestBroker;
using HI.DevOps.Application.Common.Interfaces.IRequestBroker;
using HI.DevOps.DomainCore.Enumeration.ErrorEn;
using HI.DevOps.DomainCore.Extensions;
using HI.DevOps.DomainCore.Helper.Constant;
using HI.DevOps.DomainCore.Helper.RoleBased;
using HI.DevOps.DomainCore.Models.DevOps;
using HI.DevOps.DomainCore.Models.Error;
using HI.DevOps.DomainCore.Models.Login;
using HI.DevOps.Web.Common;
using log4net;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace HI.DevOps.Web.Controllers.Authentication
{
    public class AuthenticationController : Controller
    {
        #region Constructor

        public AuthenticationController(IConfiguration iConfiguration, IMemoryCache memoryCache,
            IRequestBrokerService requestBroker, IOptions<DevOpsClient> clientSettings,IDevOpsRequestBroker devOpsRequestBroker)
        {
            _clientSettings = clientSettings;
            _iRequestBrokerService = requestBroker;
             Configuration = iConfiguration;
            _memCache = memoryCache;
            _iDevOpsRequestBrokerService = devOpsRequestBroker;
            _webLog = LogManager.GetLogger(typeof(AuthenticationController));
        }

        #endregion

        #region Member Fields

        private static ILog _webLog;
        public IConfiguration Configuration { get; }
        private readonly IMemoryCache _memCache;
        private readonly IRequestBrokerService _iRequestBrokerService;
        private readonly IDevOpsRequestBroker _iDevOpsRequestBrokerService;
        private static readonly HttpClient DevOpsHttpClient = new HttpClient();
        private static readonly Dictionary<Guid, TokenModel> AuthorizationRequests = new Dictionary<Guid, TokenModel>();
        private readonly IOptions<DevOpsClient> _clientSettings;

        #endregion

        #region Public Action Methods

        /// <summary>
        ///     Load Login screen
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult Index()
        {
            return Authorize();
        }

        /// <summary>
        /// Start a new authorization request.
        /// This creates a random state value that is used to correlate/validate the request in the callback later.
        /// </summary>
        /// <returns></returns>
        ///
        [HttpGet]
        [Route(UrlConstant.Authorize)]
        [AllowAnonymous]
        public ActionResult Authorize()
        {
            MemoryCacheHelper.SetInMemoryCache(AppConstants.DevOpsClientInfo, _clientSettings, _memCache);
            
            var state = Guid.NewGuid();

            AuthorizationRequests[state] = new TokenModel {IsPending = true};

            return new RedirectResult(GetAuthorizationUrl(state.ToString()));
        }

        /// <summary>
        /// Callback action. Invoked after the user has authorized the app.
        /// </summary>
        /// <param name="code"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult>Callback(string code, Guid state)
        {
            
            DevOpsUserProfile userInfo = null;
            if (ValidateCallbackValues(code, state.ToString(), out var error))
            {
                // Exchange the auth code for an access token and refresh token
                var requestMessage =
                    new HttpRequestMessage(HttpMethod.Post, _clientSettings.Value.TokenUrl);
                requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var form = new Dictionary<string, string>
                {
                    {"client_assertion_type", "urn:ietf:params:oauth:client-assertion-type:jwt-bearer"},
                    {"client_assertion", _clientSettings.Value.ClientSecret},
                    {"grant_type", "urn:ietf:params:oauth:grant-type:jwt-bearer"},
                    {"assertion", code},
                    {"redirect_uri", _clientSettings.Value.CallBackUrl}
                };
                requestMessage.Content = new FormUrlEncodedContent(form);

                var responseMessage = await DevOpsHttpClient.SendAsync(requestMessage);

                if (responseMessage.IsSuccessStatusCode)
                {
                    var body = await responseMessage.Content.ReadAsStringAsync();
                    
                    var tokenModel = TokenModel.FromJson(body);

                    if (tokenModel != null)
                    {
                        userInfo = _iDevOpsRequestBrokerService.GetDevOpsUserProfile(tokenModel.AccessToken);
                        MemoryCacheHelper.SetInMemoryCache(AppConstants.TokenInfo, tokenModel, _memCache);
                       
                        if (!userInfo.EmailAddress.IsNullOrEmpty())
                            MemoryCacheHelper.SetInMemoryCache(AppConstants.DevOpsUser, userInfo, _memCache);
                    }
                }
                else
                {
                    error = responseMessage.ReasonPhrase;
                }
            }
            var model = new LoginUserViewModel();
            if (!error.IsNullOrEmpty())
            {
                model.Message = "Invalid username or password.Please try again.";
                return View(UrlConstant.AuthenticationViewCshtml, model);
            }

            UserViewModel user;
            bool isValidUser;
            try
            {
                IAuthenticateBm authenticateBm = new AuthenticateBm(_iRequestBrokerService);
                model.DevOpsUserId = userInfo?.Id.ToString();
                model.UserName = userInfo?.EmailAddress;
                user = authenticateBm.AuthenticateUser(model).SourceObject as UserViewModel;
                if (user == null)
                    return NotFound();

                isValidUser = !user.Errors.HasError();
            }
            catch (Exception ex)
            {
                if (_webLog.IsErrorEnabled)
                    _webLog.Error("Error AuthenticationController-Index- Validate " + ex);
                if (ex.Message != null && ex is AppException &&
                    ex.Message.Contains(ErrorEnum.ServiceUnavailable.GetDescription()))
                    model.Message = ErrorEnum.ServiceUnavailable.GetDescription();
                else model.Message = "Login failed. Please try again.";

                return View(UrlConstant.AuthenticationViewCshtml, model);
            }

            if (user.Errors.HasError())
            {
                model.Message = user.Errors.Message;
                return View(UrlConstant.AuthenticationViewCshtml, model);
            }

            if (!user.User.UserIdentityId.IsNullOrEmpty())
            {
                if (isValidUser && !user.User.IsActive)
                {
                    model.Message = "Account lockout. Please contact administrator";
                    return View(UrlConstant.AuthenticationViewCshtml, model);
                }

                if (user.AccessToken.IsAnyNullOrEmpty())
                {
                    model.Message = "Bad Request. Please try again";
                    return View(UrlConstant.AuthenticationViewCshtml, model);
                }

                HttpContext.Session.SetString(AppConstants.SessionAccessKey, user.AccessToken.Token);
                HttpContext.Session.SetString(AppConstants.SessionUserName, userInfo?.EmailAddress);
                HttpContext.Session.SetString(AppConstants.SessionUserID, user.User.UserIdentityId);
                HttpContext.Session.SetString(AppConstants.SessionUserData, JsonConvert.SerializeObject(user.User));

                MemoryCacheHelper.SetInMemoryCache(AppConstants.SessionUser, user, _memCache);
               
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, userInfo?.DisplayName),
                    new Claim(ClaimTypes.Email, userInfo?.EmailAddress), new Claim(ClaimTypes.Role, user.User.UserRole.GetDescription()),
                    new Claim(ClaimTypes.NameIdentifier,userInfo?.Id.ToString())
                };

                Permissions.Instance.SetDefaultClaimsByRole(claims, user.User.UserRole.GetDescription());

                // complete the sign-in process
                var userIdentity = new ClaimsIdentity(claims, "login");
                var principal = new ClaimsPrincipal(userIdentity);
                await HttpContext.SignInAsync(principal);

                return RedirectToLocal(null);
            }

            return View(UrlConstant.AuthenticationViewCshtml, model);
        }

        /// <summary>
        ///     Gets a new access
        /// </summary>
        /// <param name="refreshToken"></param>
        /// <returns></returns>
        public async Task<ActionResult> RefreshToken(string refreshToken)
        {
            string error = null;
            if (!string.IsNullOrEmpty(refreshToken))
            {
                // Form the request to exchange an auth code for an access token and refresh token
                var requestMessage = new HttpRequestMessage(HttpMethod.Post, "");
                requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var form = new Dictionary<string, string>
                {
                    {"client_assertion_type", "urn:ietf:params:oauth:client-assertion-type:jwt-bearer"},
                    {"client_assertion",_clientSettings.Value.ClientSecret },
                    {"grant_type", "refresh_token"},
                    {"assertion", refreshToken},
                    {"redirect_uri", _clientSettings.Value.CallBackUrl}
                };
                requestMessage.Content = new FormUrlEncodedContent(form);

                // Make the request to exchange the auth code for an access token (and refresh token)
                var responseMessage = await DevOpsHttpClient.SendAsync(requestMessage);

                if (responseMessage.IsSuccessStatusCode)
                {
                    // Handle successful request
                    var body = await responseMessage.Content.ReadAsStringAsync();
                    var tokenModel = TokenModel.FromJson(body);
                    MemoryCacheHelper.SetInMemoryCache(AppConstants.TokenInfo, tokenModel, _memCache);
                }
                else
                {
                    error = responseMessage.ReasonPhrase;
                }
            }
            else
            {
                error = "Invalid refresh token";
            }

            if (!string.IsNullOrEmpty(error)) ViewBag.Error = error;

            return RedirectToLocal("");
        }

        #endregion

        #region Private member

        /// <summary>
        /// Constructs an authorization URL with the specified state value.
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        private  string GetAuthorizationUrl(string state)
        {
            var uriBuilder = new UriBuilder(_clientSettings.Value.AuthUrl);
            var queryParams = HttpUtility.ParseQueryString(uriBuilder.Query ?? string.Empty);

            queryParams["client_id"] = _clientSettings.Value.ClientAppId;
            queryParams["response_type"] = "Assertion";
            queryParams["state"] = state;
            queryParams["scope"] = _clientSettings.Value.Scopes;
            queryParams["redirect_uri"] = _clientSettings.Value.CallBackUrl;

            uriBuilder.Query = queryParams.ToString();

            return uriBuilder.ToString();
        }

        /// <summary>
        ///     Ensures the specified auth code and state value are valid. If both are valid, the state value is marked so it can't
        ///     be used again.
        /// </summary>
        /// <param name="code"></param>
        /// <param name="state"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        private static bool ValidateCallbackValues(string code, string state, out string error)
        {
            error = null;

            if (string.IsNullOrEmpty(code))
            {
                error = "Invalid auth code";
            }
            else
            {
                if (!Guid.TryParse(state, out var authorizationRequestKey))
                {
                    error = "Invalid authorization request key";
                }
                else
                {
                    if (!AuthorizationRequests.TryGetValue(authorizationRequestKey, out var tokenModel))
                        error = "Unknown authorization request key";
                    else if (!tokenModel.IsPending) error = "Authorization request key already used";
                    else
                        AuthorizationRequests[authorizationRequestKey].IsPending =
                            false; // mark the state value as used so it can't be reused
                }
            }

            return error == null;
        }

        /// <summary>
        /// Redirect action to local method
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (_webLog.IsDebugEnabled)
                _webLog.Debug("Entering AuthenticationController-RedirectToLocal");

            if (Url.IsLocalUrl(returnUrl))
            {
                if (_webLog.IsDebugEnabled)
                    _webLog.Debug("Exiting-AuthenticationController-RedirectToLocal--Redirecting to " + returnUrl);

                return Redirect(returnUrl);
            }

            if (_webLog.IsDebugEnabled)
                _webLog.Debug("Exiting-AuthenticationController-RedirectToLocal--Redirecting to " + returnUrl);

            return RedirectToAction("Index", "Home");
        }

        #endregion
    }
}