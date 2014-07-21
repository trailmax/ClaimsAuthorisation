using ClaimsAuth.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;


namespace ClaimsAuth.Infrastructure.Identity
{
    public class RoleManager : RoleManager<ApplicationRole>
    {
        private readonly MyDbContext context;


        public RoleManager(MyDbContext context)
            : base(new RoleStore<ApplicationRole>(context))
        {
            this.context = context;
        }
    }
}