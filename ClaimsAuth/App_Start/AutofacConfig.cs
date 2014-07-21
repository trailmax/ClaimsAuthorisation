//using System.Reflection;
//using System.Web.Mvc;
//using Autofac;
//using Autofac.Integration.Mvc;
//using ClaimsAuth.Infrastructure;
//using ClaimsAuth.Infrastructure.Identity;


//namespace ClaimsAuth
//{
//    public class AutofacConfig
//    {
//        public static void Configure()
//        {
//            var builder = new ContainerBuilder();

//            builder.RegisterType<MyDbContext>().AsSelf().InstancePerLifetimeScope();

//            builder.RegisterType<UserManager>().AsSelf().InstancePerLifetimeScope();
//            builder.RegisterType<RoleManager>().AsSelf().InstancePerLifetimeScope();
//            builder.RegisterType<ClaimedActionsProvider>().AsSelf().InstancePerLifetimeScope();


//            builder.RegisterControllers(typeof(MvcApplication).Assembly);

//            builder.RegisterModelBinders(Assembly.GetExecutingAssembly());
//            builder.RegisterModelBinderProvider();

//            builder.RegisterModule(new AutofacWebTypesModule());

//            var container = builder.Build();
//            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
//        }
//    }
//}