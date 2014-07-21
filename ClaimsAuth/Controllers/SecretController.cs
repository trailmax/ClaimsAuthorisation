using System.Web.Mvc;
using ClaimsAuth.Infrastructure.Identity;


namespace ClaimsAuth.Controllers
{
    public class SecretController : Controller
    {
        [ClaimsAuthorize("Basic Index")]
        public ActionResult Index()
        {
            return View();
        }


        [ClaimsAuthorize("Creating Something")]
        public ActionResult CreateSomething()
        {
            return View();
        }


        [ClaimsAuthorize("Edit Magic Beans")]
        public ActionResult EditMagicBeans()
        {
            return View();
        }
    }
}