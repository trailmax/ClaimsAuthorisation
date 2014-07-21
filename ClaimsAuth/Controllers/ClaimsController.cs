using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ClaimsAuth.Infrastructure.Identity;


namespace ClaimsAuth.Controllers
{
    public class ClaimsController : Controller
    {
        private readonly ClaimedActionsProvider claimedActionsProvider;


        public ClaimsController(ClaimedActionsProvider claimedActionsProvider)
        {
            this.claimedActionsProvider = claimedActionsProvider;
        }


        public ActionResult Index()
        {
            var viewModel = new ClaimsIndexViewModel()
            {
                Claims = claimedActionsProvider.GetControlledClaims(),
            };

            return View(viewModel);
        }
    }


    public class ClaimsIndexViewModel
    {
        public List<String> Claims { get; set; }
    }
}