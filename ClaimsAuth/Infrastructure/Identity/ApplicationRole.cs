using System;
using System.Collections.Generic;
using Microsoft.AspNet.Identity.EntityFramework;


namespace ClaimsAuth.Infrastructure.Identity
{
    public class ApplicationRole : IdentityRole
    {
        public const String CacheKey = "4BA44A3F-F728-42D8-9387-7577EDC0DD99_Claims_Roles_";

        public ICollection<RoleClaim> RoleClaims { get; set; }
    }
}