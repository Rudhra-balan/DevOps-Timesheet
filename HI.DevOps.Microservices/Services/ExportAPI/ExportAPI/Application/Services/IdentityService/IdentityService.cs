using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using Hi.DevOps.Export.API.DataObject.IdentityDO;
using Microsoft.AspNetCore.Http;

namespace Hi.DevOps.Export.API.Application.Services.IdentityService
{
    public class IdentityService : IIdentityService
    {
        private readonly IHttpContextAccessor _context;

        public IdentityService(IHttpContextAccessor context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public IdentityDO GetIdentity()
        {
            string authorizationHeader = _context.HttpContext.Request.Headers["Authorization"];

            if (authorizationHeader == null) throw new ArgumentNullException("accountnumber");

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = authorizationHeader.Split(" ")[1];
            var paresedToken = tokenHandler.ReadJwtToken(token);

            var account = paresedToken.Claims
                .FirstOrDefault(c => c.Type == "accountnumber");

            var name = paresedToken.Claims
                .FirstOrDefault(c => c.Type == "name");

            var currency = paresedToken.Claims
                .FirstOrDefault(c => c.Type == "currency");

            return new IdentityDO();
        }
    }
}