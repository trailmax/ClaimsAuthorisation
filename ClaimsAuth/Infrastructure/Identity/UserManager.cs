using System.Security.Claims;
using System.Threading.Tasks;
using ClaimsAuth.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;


namespace ClaimsAuth.Infrastructure.Identity
{
    public class UserManager : UserManager<ApplicationUser>
    {
        public UserManager(MyDbContext dbContext, RoleManager roleManager)
            : base(new UserStore<ApplicationUser>(dbContext))
        {
            // Configure validation logic for usernames
            this.UserValidator = new UserValidator<ApplicationUser>(this)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };
            // Configure validation logic for passwords
            this.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = false,
                RequireDigit = false,
                RequireLowercase = true,
                RequireUppercase = false,
            };
            // Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user
            // You can write your own provider and plug in here.
            this.RegisterTwoFactorProvider("PhoneCode", new PhoneNumberTokenProvider<ApplicationUser>
            {
                MessageFormat = "Your security code is: {0}"
            });
            this.RegisterTwoFactorProvider("EmailCode", new EmailTokenProvider<ApplicationUser>
            {
                Subject = "Security Code",
                BodyFormat = "Your security code is: {0}"
            });
            this.EmailService = new EmailService();
            this.SmsService = new SmsService();
            this.UserTokenProvider = new EmailTokenProvider<ApplicationUser>();
        }


        public async Task SignInAsync(IAuthenticationManager authenticationManager, ApplicationUser applicationUser, bool isPersistent)
        {
            authenticationManager.SignOut(
                DefaultAuthenticationTypes.ExternalCookie,
                DefaultAuthenticationTypes.ApplicationCookie,
                DefaultAuthenticationTypes.TwoFactorCookie,
                DefaultAuthenticationTypes.TwoFactorRememberBrowserCookie,
                DefaultAuthenticationTypes.ExternalBearer);

            var identity = await this.CreateIdentityAsync(applicationUser, DefaultAuthenticationTypes.ApplicationCookie);
            identity.AddClaim(new Claim(ClaimTypes.Email, applicationUser.Email));

            authenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, identity);
        }


        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(ApplicationUser applicationUser)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await this.CreateIdentityAsync(applicationUser, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    //public class MyClaimsIdentityFactory : ClaimsIdentityFactory<ApplicationUser, string>
    //{
    //    private readonly RoleManager roleManager;


    //    public MyClaimsIdentityFactory(RoleManager roleManager)
    //    {
    //        this.roleManager = roleManager;
    //    }


    //    public override async Task<ClaimsIdentity> CreateAsync(UserManager<ApplicationUser, string> userManager, ApplicationUser user, string authenticationType)
    //    {
    //        var claimsIdentity = await base.CreateAsync(userManager, user, authenticationType);

    //        var userRoles = await userManager.GetRolesAsync(user.Id);
    //        foreach (var role in userRoles)
    //        {
    //            var roleClaims = await roleManager.GetClaimsAsync(role);

    //            claimsIdentity.AddClaims(roleClaims);
    //        }

    //        return claimsIdentity;
    //    }
    //}
}