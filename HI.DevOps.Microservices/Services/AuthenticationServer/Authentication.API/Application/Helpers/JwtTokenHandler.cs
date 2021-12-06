using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace Hi.DevOps.Authentication.API.Application.Helpers
{
    internal static class JwtTokenHandler
    {
        private static readonly JwtSecurityTokenHandler JwtSecurityTokenHandler;


        static JwtTokenHandler()
        {
            if (JwtSecurityTokenHandler == null)
                JwtSecurityTokenHandler = new JwtSecurityTokenHandler();
        }

        public static string WriteToken(JwtSecurityToken jwt)
        {
            return JwtSecurityTokenHandler.WriteToken(jwt);
        }

        public static ClaimsPrincipal ValidateToken(string token, TokenValidationParameters tokenValidationParameters)
        {
            try
            {
                var principal =
                    JwtSecurityTokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);

                if (!(securityToken is JwtSecurityToken jwtSecurityToken) ||
                    !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                        StringComparison.InvariantCultureIgnoreCase))
                    throw new SecurityTokenException("Invalid token");

                return principal;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}