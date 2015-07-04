using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Mvc;
using ClaimsAuth.Infrastructure.Identity;


namespace ClaimsAuth.Controllers
{
    public class RolesController : Controller
    {
        private readonly RoleManager roleManager;
        private readonly ClaimedActionsProvider claimedActionsProvider;

        public RolesController(RoleManager roleManager, ClaimedActionsProvider claimedActionsProvider)
        {
            this.roleManager = roleManager;
            this.claimedActionsProvider = claimedActionsProvider;
        }


        public ActionResult Index()
        {
            var allRoles = roleManager.Roles.ToList();

            return View(allRoles);
        }


        public ActionResult Create()
        {
            return View();
        }


        [HttpPost]
        public async Task<ActionResult> Create(CreateRoleViewModel viewModel)
        {
            var newRole = new ApplicationRole()
            {
                Name = viewModel.Name,
            };
            await roleManager.CreateAsync(newRole);

            return RedirectToAction("Index");
        }


        public async Task<ActionResult> EditClaims(string id)
        {
            var role = await roleManager.FindByIdAsync(id);

            var claimGroups = claimedActionsProvider.GetClaimGroups();

            var assignedClaims = await roleManager.GetClaimsAsync(role.Name);

            var viewModel = new RoleClaimsViewModel()
            {
                RoleId = role.Id,
                RoleName = role.Name,
            };

            foreach (var claimGroup in claimGroups)
            {
                var claimGroupModel = new RoleClaimsViewModel.ClaimGroup()
                {
                    GroupId = claimGroup.GroupId,
                    GroupName = claimGroup.GroupName,
                    GroupClaimsCheckboxes = claimGroup.Claims
                        .Select(c => new SelectListItem()
                        {
                            Value = String.Format("{0}#{1}", claimGroup.GroupId, c),
                            Text = c,
                            Selected = assignedClaims.Any(ac => ac.Type == claimGroup.GroupId.ToString() && ac.Value == c)
                        }).ToList()
                };
                viewModel.ClaimGroups.Add(claimGroupModel);
            }


            return View(viewModel);
        }


        [HttpPost]
        public async Task<ActionResult> EditClaims(RoleClaimsViewModel viewModel)
        {
            var role = await roleManager.FindByIdAsync(viewModel.RoleId);
            var roleClaims = await roleManager.GetClaimsAsync(role.Name);


            // this is ugly. Deletes all the claims and adds them back in.
            // can be done in a better fashion
            foreach (var removedClaim in roleClaims)
            {
                await roleManager.RemoveClaimAsync(role.Id, removedClaim);
            }

            var submittedClaims = viewModel
                .SelectedClaims
                .Select(s =>
                    {
                        var tokens = s.Split('#');
                        if (tokens.Count() != 2)
                        {
                            throw new Exception(String.Format("Claim {0} can't be processed because it is in incorrect format", s));
                        }
                        return new Claim(tokens[0], tokens[1]);
                    }).ToList();
            

            roleClaims = await roleManager.GetClaimsAsync(role.Name);

            foreach (var submittedClaim in submittedClaims)
            {
                var hasClaim = roleClaims.Any(c => c.Value == submittedClaim.Value && c.Type == submittedClaim.Type);
                if (!hasClaim)
                {
                    await roleManager.AddClaimAsync(role.Id, submittedClaim);
                }
            }

            roleClaims = await roleManager.GetClaimsAsync(role.Name);

            var cacheKey = ApplicationRole.GetCacheKey(role.Name);
            System.Web.HttpContext.Current.Cache.Remove(cacheKey);

            return RedirectToAction("Index");
        }
    }


    public class CreateRoleViewModel
    {
        [Required]
        public String Name { get; set; }
    }


    public class RoleClaimsViewModel
    {
        public RoleClaimsViewModel()
        {
            ClaimGroups = new List<ClaimGroup>();

            SelectedClaims = new List<String>();
        }

        public String RoleId { get; set; }

        public String RoleName { get; set; }

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
}