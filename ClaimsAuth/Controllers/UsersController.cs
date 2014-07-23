using System;
using System.Collections.Generic;
using System.Data.Entity;
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
        private readonly RoleManager roleManager;


        public UsersController(UserManager userManager, ClaimedActionsProvider claimedActionsProvider, RoleManager roleManager)
        {
            this.userManager = userManager;
            this.claimedActionsProvider = claimedActionsProvider;
            this.roleManager = roleManager;
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
                var hasClaim = userClaims.Any(c => c.Value == submittedClaim && c.Type == submittedClaim);
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


        public async Task<ActionResult> EditRoles(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            var assignedRoles = await userManager.GetRolesAsync(user.Id);

            var allRoles = await roleManager.Roles.ToListAsync();

            var userRoles = allRoles.Select(r => new SelectListItem()
            {
                Value = r.Id,
                Text = r.Name,
                Selected = assignedRoles.Contains(r.Name),
            }).ToList();

            var viewModel = new UserRolesViewModel
                {
                    Username = user.UserName,
                    UserId = user.Id,
                    UserRoles = userRoles,
                };
            return View(viewModel);
        }


        [HttpPost]
        public async Task<ActionResult> EditRoles(UserRolesViewModel viewModel)
        {
            var user = await userManager.FindByIdAsync(viewModel.UserId);
            var possibleRoles = await roleManager.Roles.ToListAsync();
            var userRoles = await userManager.GetRolesAsync(user.Id);

            var submittedRoles = viewModel.SelectedRoles;

            //TODO
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


    public class UserRolesViewModel
    {
        public UserRolesViewModel()
        {
            UserRoles = new List<SelectListItem>();
            SelectedRoles = new List<String>();
        }

        public String UserId { get; set; }
        public String Username { get; set; }
        public List<SelectListItem> UserRoles { get; set; }
        public List<String> SelectedRoles { get; set; }
    }
}