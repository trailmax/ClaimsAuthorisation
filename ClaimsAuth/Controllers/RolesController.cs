using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using ClaimsAuth.Infrastructure.Identity;
using ClaimsAuth.Models;


namespace ClaimsAuth.Controllers
{
    public class RolesController : Controller
    {
        private readonly RoleManager roleManager;


        public RolesController(RoleManager roleManager)
        {
            this.roleManager = roleManager;
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


        public async Task<ActionResult> Edit(string id)
        {
            var role = await roleManager.FindByIdAsync(id);
            return View(role);
        }
    }


    public class CreateRoleViewModel
    {
        [Required]
        public String Name { get; set; }
    }
}