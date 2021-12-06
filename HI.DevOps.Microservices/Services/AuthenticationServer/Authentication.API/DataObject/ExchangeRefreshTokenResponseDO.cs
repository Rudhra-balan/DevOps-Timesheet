using Hi.DevOps.Authentication.API.Application.Helpers;

namespace Hi.DevOps.Authentication.API.DataObject
{
    public class ExchangeRefreshTokenResponseDo
    {
        public ExchangeRefreshTokenResponseDo(ErrorDo errorDo)
        {
            ErrorDo = errorDo ?? new ErrorDo();
        }

        public ExchangeRefreshTokenResponseDo(AccessToken accessToken, string refreshToken, bool success = false,
            string message = null)
        {
            Success = success;
            Message = message;
            AccessToken = accessToken;
            RefreshToken = refreshToken;
        }

        public bool Success { get; }
        public string Message { get; }
        public AccessToken AccessToken { get; }
        public string RefreshToken { get; }

        public ErrorDo ErrorDo { get; }
    }
}