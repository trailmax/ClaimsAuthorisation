using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Mvc;
using ClaimsAuth.Infrastructure.Identity;
using ClaimsAuth.Models;


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

            var possibleClaims = claimedActionsProvider.GetControlledClaims();

            var assignedClaims = await roleManager.GetClaimsAsync(role.Id);


            var roleClaims = possibleClaims.Select(pc => new SelectListItem()
            {
                Value = pc,
                Text = pc,
                Selected = assignedClaims.Select(c => c.Type).Contains(pc),
            }).ToList();


            var viewModel = new RoleClaimsViewModel()
            {
                RoleId = role.Id,
                RoleName = role.Name,
                RoleClaims = roleClaims,
            };

            return View(viewModel);
        }


        [HttpPost]
        public async Task<ActionResult> EditClaims(RoleClaimsViewModel viewModel)
        {
            var role = await roleManager.FindByIdAsync(viewModel.RoleId);

            var possibleClaims = claimedActionsProvider.GetControlledClaims();

            var roleClaims = await roleManager.GetClaimsAsync(role.Id);

            var submittedClaims = viewModel.SelectedClaims.ToList();

            foreach (var submittedClaim in submittedClaims)
            {
                var hasClaim = roleClaims.Any(c => c.Value == submittedClaim && c.Type == submittedClaim);
                if (!hasClaim)
                {
                    await roleManager.AddClaimAsync(role.Id, new Claim(submittedClaim, submittedClaim));
                }
            }

            foreach (var removedClaim in possibleClaims.Except(submittedClaims))
            {
                await roleManager.RemoveClaimAsync(role.Id, new Claim(removedClaim, removedClaim));
            }

            roleClaims = await roleManager.GetClaimsAsync(role.Id);

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
            RoleClaims = new List<SelectListItem>();

            SelectedClaims = new List<String>();
        }

        public String RoleId { get; set; }

        public String RoleName { get; set; }

        public List<SelectListItem> RoleClaims { get; set; }

        public IEnumerable<String> SelectedClaims { get; set; }
    }
}