using System;
using System.Threading.Tasks;
using Hi.DevOps.Authentication.API.Application.BussinessManagerInterface;
using Hi.DevOps.Authentication.API.DataObject;
using Microsoft.AspNetCore.Mvc;

namespace Hi.DevOps.Authentication.API.Controllers
{
    public class AuthenticationController : BaseController
    {
        private readonly IJwtFactoryBm _jwtFactory;

        public AuthenticationController(IJwtFactoryBm jwtFactory)
        {
            _jwtFactory = jwtFactory;
        }

        [HttpPost("AuthenticateUser")]
        public async Task<IActionResult> AuthenticateUser([FromBody] LoginRequestDo loginDo)
        {
            if (loginDo == null) throw new ArgumentNullException(nameof(loginDo));
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (string.IsNullOrEmpty(loginDo.Username) || string.IsNullOrEmpty(loginDo.DevOpsUserId))
                return BadRequest(new {message = "Username or password is cannot be empty"});
            var token = await _jwtFactory.AuthenticateAsync(loginDo.Username,  loginDo.DevOpsUserId).ConfigureAwait(false);
            if (token == null) return BadRequest(new {message = "Username or password is incorrect"});
            return Ok(token);
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult> RefreshToken(
            [FromBody] ExchangeRefreshTokenRequestDo exchangeRefreshTokenRequestDo)
        {
            if (exchangeRefreshTokenRequestDo == null)
                throw new ArgumentNullException(nameof(exchangeRefreshTokenRequestDo));
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (string.IsNullOrEmpty(exchangeRefreshTokenRequestDo.AccessToken) ||
                string.IsNullOrEmpty(exchangeRefreshTokenRequestDo.RefreshToken))
                return BadRequest(new {message = "Invalid Request"});
            var refreshToken = await _jwtFactory.RefreshTokenAsync(exchangeRefreshTokenRequestDo).ConfigureAwait(false);
            if (refreshToken == null) return BadRequest(new {message = "Invalid Token"});
            return Ok(refreshToken);
        }

    }
}