using System.Collections.Generic;
using Microsoft.AspNet.Identity.EntityFramework;


namespace ClaimsAuth.Infrastructure.Identity
{
    public class ApplicationRole : IdentityRole
    {
        public ICollection<RoleClaim> RoleClaims { get; set; }
    }
}