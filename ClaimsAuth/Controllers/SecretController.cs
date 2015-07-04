using System.Web.Mvc;
using ClaimsAuth.Infrastructure.Identity;


namespace ClaimsAuth.Controllers
{
    [ClaimsGroup(ClaimResources.Secrets)]
    public class SecretController : Controller
    {
        [ClaimsAction(ClaimsActions.Index)]
        public ActionResult Index()
        {
            return View();
        }


        [ClaimsAction(ClaimsActions.Create)]
        [ClaimsAction(ClaimsActions.Index)]
        public ActionResult CreateSomething()
        {
            return View();
        }


        [ClaimsAction(ClaimsActions.Delete)]
        public ActionResult EditMagicBeans()
        {
            return View();
        }
    }
}