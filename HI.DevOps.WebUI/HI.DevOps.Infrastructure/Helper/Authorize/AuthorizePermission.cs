using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace HI.DevOps.Infrastructure.Helper.Authorize
{
    public class AuthorizePermission : AuthorizeAttribute, IAuthorizationFilter
    {
        public string Permissions { get; set; }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (string.IsNullOrEmpty(Permissions))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            if (context.HttpContext.User.Identity.IsAuthenticated)
                if (context.HttpContext.User.Identities.ToList().Any())
                    foreach (var claimsIdentity in context.HttpContext.User.Identities.ToList())
                    {
                        if (claimsIdentity == null) continue;
                        var permissionsFromToken =
                            claimsIdentity.Claims.ToList();
                        var requiredPermissions =
                            Permissions.Split(
                                ",");
                        if (requiredPermissions.Any(x => permissionsFromToken.Any(claim => claim.Value.Contains(x))))
                            return;
                    }

            context.Result = new UnauthorizedResult();
        }
    }
}