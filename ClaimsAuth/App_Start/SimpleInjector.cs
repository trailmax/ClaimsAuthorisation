using System.Reflection;
using System.Web;
using System.Web.Mvc;
using ClaimsAuth.Infrastructure;
using ClaimsAuth.Infrastructure.Identity;
using SimpleInjector;
using SimpleInjector.Integration.Web.Mvc;


namespace ClaimsAuth
{
    public static class SimpleInjector
    {
        public static void Configure()
        {
            var container = new Container();

            container.RegisterPerWebRequest<MyDbContext>();

            container.RegisterPerWebRequest<UserManager>();

            container.RegisterPerWebRequest<RoleManager>();

            container.RegisterPerWebRequest<ClaimedActionsProvider>();

            container.Register<SignInManager>();

            container.RegisterPerWebRequest(() => HttpContext.Current.GetOwinContext().Authentication);

            container.RegisterMvcControllers(Assembly.GetExecutingAssembly());


            container.RegisterMvcIntegratedFilterProvider();

            DependencyResolver.SetResolver(new SimpleInjectorDependencyResolver(container));
        }
    }
}