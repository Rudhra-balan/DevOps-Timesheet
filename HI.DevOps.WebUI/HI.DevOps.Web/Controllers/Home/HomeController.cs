using HI.DevOps.DomainCore.Helper.RoleBased;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HI.DevOps.Web.Controllers.Home
{
    [Authorize(Permissions.AdminPolicy)]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}