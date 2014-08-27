using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web.Mvc;
using ClaimsAuth.Infrastructure.Identity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using System;
using ClaimsAuth.Models;


namespace ClaimsAuth
{
    public partial class Startup
    {
        // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {
            var cookieAuthenticationProvider = new CookieAuthenticationProvider();
            cookieAuthenticationProvider.OnValidateIdentity = async context =>
            {
                SecurityStampValidator.OnValidateIdentity<UserManager, ApplicationUser>(
                    TimeSpan.FromMinutes(0), (manager, user) => manager.GenerateUserIdentityAsync(user));

                var userId = context.Identity.GetUserId();
                if (userId == null)
                {
                    return;
                }

                // get list of roles on the user
                var userRoles = context.Identity
                    .Claims
                    .Where(c => c.Type == ClaimTypes.Role)
                    .Select(c => c.Value)
                    .ToList();

                foreach (var roleName in userRoles)
                {
                    var cacheKey = ApplicationRole.GetCacheKey(roleName);
                    var cachedClaims = System.Web.HttpContext.Current.Cache[cacheKey] as IEnumerable<Claim>;
                    if (cachedClaims == null)
                    {
                        var roleManager = DependencyResolver.Current.GetService<RoleManager>();
                        cachedClaims = await roleManager.GetClaimsAsync(roleName);
                        System.Web.HttpContext.Current.Cache[cacheKey] = cachedClaims;
                    }
                    context.Identity.AddClaims(cachedClaims);
                }

            };
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login"),
                Provider = cookieAuthenticationProvider,
                CookieName = "jumpingjacks",
                CookieHttpOnly = true,
            });
        }
    }
}