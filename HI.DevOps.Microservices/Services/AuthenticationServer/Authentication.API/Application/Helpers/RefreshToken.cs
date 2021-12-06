using System;

namespace Hi.DevOps.Authentication.API.Application.Helpers
{
    public class RefreshToken
    {
        public RefreshToken(string token, DateTime expires, int userId, string remoteIpAddress)
        {
            Token = token;
            Expires = expires;
            UserId = userId;
            RemoteIpAddress = remoteIpAddress;
        }

        public string Token { get; }
        public DateTime Expires { get; }
        public int UserId { get; }
        public bool Active => DateTime.UtcNow <= Expires;
        public string RemoteIpAddress { get; }
    }
}