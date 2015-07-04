using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ClaimsAuth.Controllers
{
    [Authorize]
    public class AuthenticatedController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}