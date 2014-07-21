using System.Reflection;
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

            container.Register<MyDbContext>(Lifestyle.Transient);

            container.Register<UserManager>(Lifestyle.Transient);

            container.Register<RoleManager>(Lifestyle.Transient);

            container.Register<ClaimedActionsProvider>(Lifestyle.Transient);

            container.RegisterMvcControllers(Assembly.GetExecutingAssembly());

            container.RegisterMvcIntegratedFilterProvider();

            // 3. Optionally verify the container's configuration.
            container.Verify();

            DependencyResolver.SetResolver(new SimpleInjectorDependencyResolver(container));
        }
    }
}