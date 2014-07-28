using System;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using ClaimsAuth.Infrastructure.Identity;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;


namespace ClaimsAuth.Controllers
{
    public class ProfilerController : Controller
    {
        private readonly UserManager userManager;


        public ProfilerController(UserManager userManager)
        {
            this.userManager = userManager;
        }


        public ActionResult Index()
        {
            return View();
        }


        public ActionResult AddClaims()
        {
            return View();
        }


        [HttpPost]
        public async Task<ActionResult> AddClaims(AddProfileClaimsViewModel viewModel)
        {
            var userId = ClaimsPrincipal.Current.Identity.GetUserId();
            var user = await userManager.FindByIdAsync(userId);

            for (int i = 0; i < viewModel.NumberOfClaims; i++)
            {
                var result =
                    await userManager.AddClaimAsync(userId, new Claim(viewModel.ClaimType, viewModel.ClaimValue));
            }

            await userManager.SignInAsync(AuthenticationManager, user, true);


            return RedirectToAction("Index");
        }


        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }


    }


    public class AddProfileClaimsViewModel
    {
        public String ClaimType { get; set; }
        public String ClaimValue { get; set; }
        public int NumberOfClaims { get; set; }
    }
}