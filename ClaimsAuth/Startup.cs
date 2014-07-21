using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ClaimsAuth.Startup))]
namespace ClaimsAuth
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
