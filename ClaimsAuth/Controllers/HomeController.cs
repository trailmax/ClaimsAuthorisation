using System.Security.Claims;
using System.Web.Mvc;

namespace ClaimsAuth.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult AddNonPersistedClaim()
        {
            var identity = (ClaimsIdentity)ClaimsPrincipal.Current.Identity;
            identity.AddClaim(new Claim("Hello", "Non Persisted Identity"));

            return RedirectToAction("Index", "Profiler");
        }
    }
}