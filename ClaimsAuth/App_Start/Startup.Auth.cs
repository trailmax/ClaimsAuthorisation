using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Mvc;
using ClaimsAuth.Infrastructure;
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
            // need to add UserManager into owin, because this is used in cookie invalidation
            app.CreatePerOwinContext(() => DependencyResolver.Current.GetService<UserManager>());

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login"),
                //Provider = GetStandardCookieAuthenticationProvider(),
                Provider = GetMyCookieAuthenticationProvider(),
                CookieName = "jumpingjacks",
                CookieHttpOnly = true,
            });
        }



        /// <summary>
        /// Cookie auth provider that adds extra role claims on the identity
        /// Role claims are kept in cache and added on the identity on every request
        /// </summary>
        /// <returns></returns>
        private static CookieAuthenticationProvider GetMyCookieAuthenticationProvider()
        {
            var cookieAuthenticationProvider = new CookieAuthenticationProvider();
            cookieAuthenticationProvider.OnValidateIdentity = async context =>
            {
                var cookieValidatorFunc = SecurityStampValidator.OnValidateIdentity<UserManager, ApplicationUser>(
                    TimeSpan.FromMinutes(10),
                    (manager, user) =>
                    {
                        var identity = manager.GenerateUserIdentityAsync(user);
                        return identity;
                    });
                await cookieValidatorFunc.Invoke(context);

                if (context.Identity == null || !context.Identity.IsAuthenticated)
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
            return cookieAuthenticationProvider;
        }


        /// <summary>
        /// This is run of the mill cookie authentication provider
        /// with invalidating the cookie on security stamp change every 10 minutes
        /// </summary>
        /// <returns></returns>
        private static CookieAuthenticationProvider GetStandardCookieAuthenticationProvider()
        {
            return new CookieAuthenticationProvider()
            {
                OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<UserManager, ApplicationUser>(
                    TimeSpan.FromMinutes(10),
                    (manager, user) =>
                    {
                        var identity = manager.GenerateUserIdentityAsync(user);
                        return identity;
                    })
            };
        }
    }
}