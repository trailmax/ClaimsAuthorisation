using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Mvc;
using ClaimsAuth.Infrastructure.Identity;
using ClaimsAuth.Models;


namespace ClaimsAuth.Controllers
{
    public class UsersController : Controller
    {
        private readonly UserManager userManager;
        private readonly ClaimedActionsProvider claimedActionsProvider;


        public UsersController(UserManager userManager, ClaimedActionsProvider claimedActionsProvider)
        {
            this.userManager = userManager;
            this.claimedActionsProvider = claimedActionsProvider;
        }


        public ActionResult Index()
        {
            var viewModel = new UsersIndexViewIndex()
            {
                Users = userManager.Users.ToList(),
            };

            return View(viewModel);
        }


        public async Task<ActionResult> EditClaims(String id)
        {
            var user = await userManager.FindByIdAsync(id);

            var possibleClaims = claimedActionsProvider.GetControlledClaims();

            var assignedClaims = await userManager.GetClaimsAsync(user.Id);

            var userClaims = possibleClaims.Select(pc => new SelectListItem()
            {
                Value = pc,
                Text = pc,
                Selected = assignedClaims.Select(c => c.Type).Contains(pc),
            }).ToList();

            var viewModel = new UserClaimsViewModel()
            {
                Username = user.UserName,
                UserId = user.Id,
                UserClaims = userClaims,
            };

            return View(viewModel);
        }


        [HttpPost]
        public async Task<ActionResult> EditClaims(UserClaimsViewModel viewModel)
        {
            var user = await userManager.FindByIdAsync(viewModel.UserId);

            var possibleClaims = claimedActionsProvider.GetControlledClaims();

            var userClaims = await userManager.GetClaimsAsync(user.Id);

            var submittedClaims = viewModel.SelectedClaims.ToList();

            var shouldUpdateSecurityStamp = false;

            foreach (var submittedClaim in submittedClaims)
            {
                var hasClaim = userClaims.Contains(new Claim(submittedClaim, submittedClaim));
                if (!hasClaim)
                {
                    await userManager.AddClaimAsync(user.Id, new Claim(submittedClaim, submittedClaim));
                    shouldUpdateSecurityStamp = true;
                }
            }

            foreach (var removedClaim in possibleClaims.Except(submittedClaims))
            {
                await userManager.RemoveClaimAsync(user.Id, new Claim(removedClaim, removedClaim));
                shouldUpdateSecurityStamp = true;
            }

            if (shouldUpdateSecurityStamp)
            {
                await userManager.UpdateSecurityStampAsync(user.Id);
            }

            userClaims = await userManager.GetClaimsAsync(user.Id);

            return RedirectToAction("Index");
        }
    }


    public class UsersIndexViewIndex
    {
        public List<ApplicationUser> Users { get; set; }
    }


    public class UserClaimsViewModel
    {
        public UserClaimsViewModel()
        {
            UserClaims = new List<SelectListItem>();
            SelectedClaims = new List<String>();
        }

        public String UserId { get; set; }
        public String Username { get; set; }
        public List<SelectListItem> UserClaims { get; set; }
        public IEnumerable<String> SelectedClaims { get; set; }
    }
}