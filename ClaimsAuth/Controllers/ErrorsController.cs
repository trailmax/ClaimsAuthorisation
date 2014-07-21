using System.Web.Mvc;

namespace ClaimsAuth.Controllers
{
    public class ErrorsController : Controller
    {
        public ActionResult Unauthorised()
        {
            return View();
        }
    }
}