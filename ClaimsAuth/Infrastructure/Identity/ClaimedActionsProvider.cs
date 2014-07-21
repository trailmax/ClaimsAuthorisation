using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;


namespace ClaimsAuth.Infrastructure.Identity
{
    public class ClaimedActionsProvider
    {
        public List<String> GetControlledClaims()
        {
            var claimedActions = GetControllerTypes()
                .SelectMany(ct => ct.GetMethods())
                .Where(m => m.IsDefined(typeof(ClaimsAuthorizeAttribute)))
                .Select(m => m.GetCustomAttribute<ClaimsAuthorizeAttribute>())
                .Select(a => a.Name)
                .ToList();

            return claimedActions;
        }


        public static IEnumerable<Type> GetControllerTypes()
        {
            return Assembly.GetAssembly(typeof(MvcApplication))
                .GetTypes()
                .Where(t => !t.IsAbstract && t.IsSubclassOf(typeof(Controller)))
                .Where(t => t.Namespace != null && !t.Name.Contains("T4MVC"))
                .ToList();
        }

    }
}