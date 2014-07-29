using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
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
            var cookies = Request.Cookies;
            var claims = ClaimsPrincipal.Current.Claims.ToList();

            var claimsTextLength = claims.Sum(c => c.Type.Length + c.Value.Length);


            var appCookie = cookies[".AspNet.ApplicationCookie"] ?? new HttpCookie("No App Cookie!", "I said nothing!");

            var appCookieLength = appCookie.Value.Length;

            var model = new ProfilerIndexViewModel()
            {
                TotalClaimsLength = claimsTextLength,
                AppCookieLength = appCookieLength,
                CookiePerClaims = (float)(appCookieLength / (claimsTextLength + 0.001)),
                Cookies = cookies,
                Claims = claims,
            };



            return View(model);
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

            await userManager.AddClaimAsync(userId, new Claim(viewModel.ClaimType, viewModel.ClaimValue));

            await userManager.SignInAsync(AuthenticationManager, user, true);

            return RedirectToAction("Index");
        }


        public ActionResult AddRandomClaims()
        {
            return View();
        }


        [HttpPost]
        public async Task<ActionResult> AddRandomClaims(AddRandomClaims model)
        {
            var userId = ClaimsPrincipal.Current.Identity.GetUserId();
            var user = await userManager.FindByIdAsync(userId);

            for (int i = 0; i < model.NumberOfClaims; i++)
            {
                var claim = GenerateRandomString(model.LengthOfClaimName);
                var claimValue = GenerateRandomString(model.LengthOfValue);

                await userManager.AddClaimAsync(userId, new Claim(claim, claimValue));
            }


            await userManager.SignInAsync(AuthenticationManager, user, true);

            return RedirectToAction("Index");
        }


        [HttpPost]
        public async Task<ActionResult> DeleteAllClaims()
        {
            var userId = ClaimsPrincipal.Current.Identity.GetUserId();
            var user = await userManager.FindByIdAsync(userId);

            var claims = await userManager.GetClaimsAsync(userId);

            foreach (var claim in claims)
            {
                await userManager.RemoveClaimAsync(userId, claim);
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


        private static String GenerateRandomString(int length)
        {
            RandomNumberGenerator rng = new RNGCryptoServiceProvider();
            var tokenData = new byte[2*length];
            rng.GetBytes(tokenData);

            var token = Convert.ToBase64String(tokenData);
            return token.Substring(0, length);
        }
    }



    public class AddProfileClaimsViewModel
    {
        public String ClaimType { get; set; }
        public String ClaimValue { get; set; }
    }

    public class AddRandomClaims
    {
        [Display(Name = "Number Of Claims")]
        public int NumberOfClaims { get; set; }

        [Display(Name = "Length Of Claim Name")]
        public int LengthOfClaimName { get; set; }

        [Display(Name = "Length of Claim Value")]
        public int LengthOfValue { get; set; }
    }


    public class ProfilerIndexViewModel
    {
        public int AppCookieLength { get; set; }

        public int TotalClaimsLength { get; set; }

        public float CookiePerClaims { get; set; }

        public HttpCookieCollection Cookies { get; set; }

        public List<Claim> Claims { get; set; }
    }
}