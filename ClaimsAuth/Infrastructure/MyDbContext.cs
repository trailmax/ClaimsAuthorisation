using System.Data.Entity;
using ClaimsAuth.Infrastructure.Configuration;
using ClaimsAuth.Infrastructure.Identity;
using ClaimsAuth.Models;
using Microsoft.AspNet.Identity.EntityFramework;


namespace ClaimsAuth.Infrastructure
{
    public class MyDbContext : IdentityDbContext<ApplicationUser>
    {
        public MyDbContext()
            : base(ConfigurationContext.Current.GetDatabaseConnectionString(), throwIfV1Schema: false)
        {
        }


        public IDbSet<RoleClaim> RoleClaims { get; set; }
    }
}