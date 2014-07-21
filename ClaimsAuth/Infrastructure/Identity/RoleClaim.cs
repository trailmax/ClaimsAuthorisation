using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNet.Identity.EntityFramework;


namespace ClaimsAuth.Infrastructure.Identity
{
    [Table("AspNetRoleClaims")]
    public class RoleClaim
    {
        [Key]
        public String Id { get; set; }

        public String RoleId { get; set; }
        [ForeignKey("RoleId")]
        public IdentityRole IdentityRole { get; set; }

        public String ClaimType { get; set; }
        public String ClaimValue { get; set; }
    }
}