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
            // Configure the db context and user manager to use a single instance per request
            //app.CreatePerOwinContext(MyDbContext.Create);
            //app.CreatePerOwinContext<UserManager>(UserManager.Create);

            // Enable the application to use a cookie to store information for the signed in user
            // and to use a cookie to temporarily store information about a user logging in with a third party login provider
            // Configure the sign in cookie
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login"),
                Provider = new CookieAuthenticationProvider
                {
                    OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<UserManager, ApplicationUser>(
                        validateInterval: TimeSpan.FromMinutes(0),
                        regenerateIdentity: (manager, user) => manager.GenerateUserIdentityAsync(user))
                },
                CookieName = "jumpingjacks",
                CookieHttpOnly = true,
            });
            
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);
        }
    }
}