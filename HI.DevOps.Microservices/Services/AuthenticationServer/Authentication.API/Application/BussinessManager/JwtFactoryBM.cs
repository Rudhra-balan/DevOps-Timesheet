using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Hi.DevOps.Authentication.API.Application.BussinessManagerInterface;
using Hi.DevOps.Authentication.API.Application.Constants;
using Hi.DevOps.Authentication.API.Application.DatabaseRepoInterface;
using Hi.DevOps.Authentication.API.Application.Helpers;
using Hi.DevOps.Authentication.API.Common;
using Hi.DevOps.Authentication.API.Common.Enum;
using Hi.DevOps.Authentication.API.DataObject;
using log4net;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using static Hi.DevOps.Authentication.API.Application.Constants.Constants.Strings;
using ApplicationException = Hi.DevOps.Authentication.API.Common.Exception.ApplicationException;

namespace Hi.DevOps.Authentication.API.Application.BussinessManager
{
    public class JwtFactoryBm : IJwtFactoryBm
    {
        #region constructor

        public JwtFactoryBm(IOptions<AppSettings> appSettings, IOptions<JwtIssuerOptions> jwtOptions,
            IJwtFactoryRepo jwtFactoryRepo)
        {
            Settings = appSettings.Value;
            JwtOptions = jwtOptions.Value;
            JwtFactoryRepo = jwtFactoryRepo;
            SysLog = LogManager.GetLogger(typeof(JwtFactoryBm));
            ThrowIfInvalidOptions(JwtOptions);
        }

        #endregion

        #region Private variable

        private AppSettings Settings { get; }
        private JwtIssuerOptions JwtOptions { get; }
        private IJwtFactoryRepo JwtFactoryRepo { get; }
        public ILog SysLog { get; }

        #endregion

        #region Public

        public async Task<UserResponseDo> AuthenticateAsync(string username, string devOpsUserDescriptor)
        {
            string refreshToken;
            AccessToken accessToken;
            User userInfo;
            if (SysLog.IsDebugEnabled)
                SysLog.Debug(string.Format(ErrorMessageConstants.LogEnteringMethodInfo,
                    $"{typeof(JwtFactoryBm)}_{MethodBase.GetCurrentMethod()}"));
            try
            {
                if (JwtFactoryRepo == null) return null;
                if(devOpsUserDescriptor.IsNullOrEmpty()) return new UserResponseDo(new ErrorDo
                {
                    Id = (int)ErrorEnum.UserAuthenticateError,
                    Message = ErrorEnum.UserAuthenticateError.GetDescription()
                });

                userInfo = JwtFactoryRepo.GetUserInformation(username);
                if (userInfo.UserIdentityId.IsNullOrEmpty())
                    return new UserResponseDo(new ErrorDo
                    {
                        Id = (int) ErrorEnum.UserAuthenticateError,
                        Message = ErrorEnum.UserAuthenticateError.GetDescription()
                    });
                refreshToken = GenerateToken();
                accessToken = await GenerateEncodedToken(userInfo.UserIdentityId, username).ConfigureAwait(false);
                if (accessToken == null)
                    return new UserResponseDo(new ErrorDo
                    {
                        Id = (int) ErrorEnum.UserAuthenticateError,
                        Message = ErrorEnum.UserAuthenticateError.GetDescription()
                    });

            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred Generating RefreshTokenAsync",
                    ErrorEnum.InvalidRefreshToken, ex);
            }

            if (SysLog.IsDebugEnabled)
                SysLog.Debug(string.Format(ErrorMessageConstants.LogExitingMethodInfo,
                    $"{typeof(JwtFactoryBm)}_{MethodBase.GetCurrentMethod()}"));
           
            return new UserResponseDo(accessToken, refreshToken, userInfo, "Successfully login");
        }

        public async Task<ExchangeRefreshTokenResponseDo> RefreshTokenAsync(
            ExchangeRefreshTokenRequestDo exchangeRefreshTokenRequestDo)
        {
            if (SysLog.IsDebugEnabled)
                SysLog.Debug(string.Format(ErrorMessageConstants.LogEnteringMethodInfo,
                    $"{typeof(JwtFactoryBm)}_{MethodBase.GetCurrentMethod()}"));
            try
            {
                if (exchangeRefreshTokenRequestDo.IsAnyNullOrEmpty())
                    return new ExchangeRefreshTokenResponseDo(new ErrorDo
                    {
                        Id = (int) ErrorEnum.InvalidRefreshToken,
                        Message = ErrorEnum.InvalidRefreshToken.GetDescription(),
                        Type = ErrorTypeEnum.Error
                    });
                var claimsPrincipal = GetPrincipalFromToken(exchangeRefreshTokenRequestDo.AccessToken, Settings.Secret);
                var id = claimsPrincipal?.Claims.First(c => c.Type == "id");
                if (id != null)
                {
                    var jwtToken = await GenerateEncodedToken(exchangeRefreshTokenRequestDo.UserId,
                        exchangeRefreshTokenRequestDo.Username).ConfigureAwait(false);
                    var refreshToken = GenerateToken();
                    if (jwtToken == null || refreshToken.IsNullOrEmpty())
                        return new ExchangeRefreshTokenResponseDo(new ErrorDo
                        {
                            Id = (int) ErrorEnum.InvalidRefreshToken,
                            Message = ErrorEnum.InvalidRefreshToken.GetDescription(),
                            Type = ErrorTypeEnum.Error
                        });
                    if (!JwtFactoryRepo.UpdateRefreshToken(exchangeRefreshTokenRequestDo.RefreshToken))
                        return new ExchangeRefreshTokenResponseDo(new ErrorDo
                        {
                            Id = (int) ErrorEnum.InvalidRefreshToken,
                            Message = ErrorEnum.InvalidRefreshToken.GetDescription(),
                            Type = ErrorTypeEnum.Error
                        });
                    return new ExchangeRefreshTokenResponseDo(jwtToken, refreshToken, true);
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred Generating RefreshTokenAsync",
                    ErrorEnum.InvalidRefreshToken, ex);
            }

            if (SysLog.IsDebugEnabled)
                SysLog.Debug(string.Format(ErrorMessageConstants.LogExitingMethodInfo,
                    $"{typeof(JwtFactoryBm)}_{MethodBase.GetCurrentMethod()}"));
            return new ExchangeRefreshTokenResponseDo(new ErrorDo
            {
                Id = (int) ErrorEnum.InvalidRefreshToken,
                Message = ErrorEnum.InvalidRefreshToken.GetDescription(),
                Type = ErrorTypeEnum.Error
            });
        }

        #endregion

        #region Private method

        private ClaimsPrincipal GetPrincipalFromToken(string token, string signingKey)
        {
            return JwtTokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey)),
                ValidateLifetime = false // we check expired tokens here
            });
        }

        private async Task<AccessToken> GenerateEncodedToken(string id, string userName)
        {
            var identity = GenerateClaimsIdentity(id, userName);
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userName),
                new Claim(JwtRegisteredClaimNames.Jti, await JwtOptions.JtiGenerator()),
                new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(JwtOptions.IssuedAt).ToString(),
                    ClaimValueTypes.Integer64),
                identity.FindFirst(JwtClaimIdentifiers.Role), identity.FindFirst(JwtClaimIdentifiers.Id)
            };

            // Create the JWT security token and encode it.
            var jwt = new JwtSecurityToken(JwtOptions.Issuer, JwtOptions.Audience, claims, JwtOptions.NotBefore,
                JwtOptions.Expiration, JwtOptions.SigningCredentials);
            return new AccessToken(JwtTokenHandler.WriteToken(jwt), (int) JwtOptions.ValidFor.TotalSeconds);
        }

        private static ClaimsIdentity GenerateClaimsIdentity(string id, string userName)
        {
            return new ClaimsIdentity(new GenericIdentity(userName, "Token"),
                new[]
                {
                    new Claim(JwtClaimIdentifiers.Id, id), new Claim(JwtClaimIdentifiers.Role, JwtClaims.ApiAccess)
                });
        }

        private static long ToUnixEpochDate(DateTime date)
        {
            return (long) Math.Round((date.ToUniversalTime() - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero))
                .TotalSeconds);
        }

        private string GenerateToken(int size = 32)
        {
            var randomNumber = new byte[size];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        private static void ThrowIfInvalidOptions(JwtIssuerOptions options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));
            if (options.ValidFor <= TimeSpan.Zero)
                throw new ArgumentException("Must be a non-zero TimeSpan.", nameof(JwtIssuerOptions.ValidFor));
            if (options.SigningCredentials == null)
                throw new ArgumentNullException(nameof(JwtIssuerOptions.SigningCredentials));
            if (options.JtiGenerator == null) throw new ArgumentNullException(nameof(JwtIssuerOptions.JtiGenerator));
        }

        #endregion
    }
}