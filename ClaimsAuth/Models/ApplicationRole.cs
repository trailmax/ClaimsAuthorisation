using System.Collections.Generic;
using ClaimsAuth.Infrastructure.Identity;
using Microsoft.AspNet.Identity.EntityFramework;


namespace ClaimsAuth.Models
{
    public class ApplicationRole : IdentityRole
    {
        public ICollection<RoleClaim> RoleClaims { get; set; }
    }
}