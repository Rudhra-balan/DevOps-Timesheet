using Hi.DevOps.Authentication.API.Application.Helpers;

namespace Hi.DevOps.Authentication.API.DataObject
{
    public class UserResponseDo
    {
        public UserResponseDo(ErrorDo errors)
        {
            Errors = errors ?? new ErrorDo();
        }

        public UserResponseDo(AccessToken accessToken, string refreshToken, User user, string message = null)
        {
            AccessToken = accessToken;
            RefreshToken = refreshToken;
            User = user;
            Message = message;
        }

        public User User { get; }
        public AccessToken AccessToken { get; }
        public string RefreshToken { get; }
        public ErrorDo Errors { get; }

        public string Message { get; }
    }
}