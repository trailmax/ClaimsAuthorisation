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

        public async Task<ActionResult> ViewClaims(String id)
        {
            var user = await userManager.FindByIdAsync(id);

            var userRoles = await userManager.GetRolesAsync(id);
            var userclaims = new List<Claim>();
            foreach (var role in userRoles)
            {
                var roleClaims = await roleManager.GetClaimsAsync(role);

                userclaims.AddRange(roleClaims);
            }

            var claimGroups = claimedActionsProvider.GetClaimGroups();

            var viewModel = new UserClaimsViewModel()
            {
                UserName = user.UserName,
            };

            foreach (var claimGroup in claimGroups)
            {
                var claimGroupModel = new UserClaimsViewModel.ClaimGroup()
                {
                    GroupId = claimGroup.GroupId,
                    GroupName = claimGroup.GroupName,
                    GroupClaimsCheckboxes = claimGroup.Claims
                        .Select(c => new SelectListItem()
                        {
                            Value = String.Format("{0}#{1}", claimGroup.GroupId, c),
                            Text = c,
                            Selected = userclaims.Any(ac => ac.Type == claimGroup.GroupId.ToString() && ac.Value == c)
                        }).ToList()
                };
                viewModel.ClaimGroups.Add(claimGroupModel);
            }

            return View(viewModel);
        }


        public async Task<ActionResult> EditRoles(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            var assignedRoles = await userManager.GetRolesAsync(user.Id);

            var allRoles = await roleManager.Roles.ToListAsync();

            var userRoles = allRoles.Select(r => new SelectListItem()
            {
                Value = r.Name,
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

            var shouldUpdateSecurityStamp = false;

            foreach (var submittedRole in submittedRoles)
            {
                var hasRole = await userManager.IsInRoleAsync(user.Id, submittedRole);
                if (!hasRole)
                {
                    shouldUpdateSecurityStamp = true;
                    await userManager.AddToRoleAsync(user.Id, submittedRole);
                }
            }

            foreach (var removedRole in possibleRoles.Select(r => r.Name).Except(submittedRoles))
            {
                shouldUpdateSecurityStamp = true;
                await userManager.RemoveFromRoleAsync(user.Id, removedRole);
            }

            if (shouldUpdateSecurityStamp)
            {
                await userManager.UpdateSecurityStampAsync(user.Id);
            }

            return RedirectToAction("Index");
        }


        public async Task<ActionResult> UpdateSecurityStamp(String userId)
        {
            await userManager.UpdateSecurityStampAsync(userId);

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
            ClaimGroups = new List<ClaimGroup>();

            SelectedClaims = new List<String>();
        }

        public String UserName { get; set; }

        public List<ClaimGroup> ClaimGroups { get; set; }

        public IEnumerable<String> SelectedClaims { get; set; }


        public class ClaimGroup
        {
            public ClaimGroup()
            {
                GroupClaimsCheckboxes = new List<SelectListItem>();
            }
            public String GroupName { get; set; }

            public int GroupId { get; set; }

            public List<SelectListItem> GroupClaimsCheckboxes { get; set; }
        }
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