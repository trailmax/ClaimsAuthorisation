using System;
using ClaimsAuth.Models;
using Microsoft.Owin.Security;


namespace ClaimsAuth.Infrastructure.Identity
{
    public class SignInManager : Microsoft.AspNet.Identity.Owin.SignInManager<ApplicationUser, String>
    {
        public SignInManager(UserManager userManager, IAuthenticationManager authenticationManager) : base(userManager, authenticationManager)
        {
        }
    }
}