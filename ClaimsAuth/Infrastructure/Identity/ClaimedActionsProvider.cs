using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;


namespace ClaimsAuth.Infrastructure.Identity
{
    public class ClaimedActionsProvider
    {
        public List<ClaimsGroup> GetControlledClaims()
        {
            //TODO FIX
            throw new NotImplementedException();
            //var claimedGroups = Assembly.GetAssembly(typeof(MvcApplication))
            //    .GetTypes()
            //    .Where(t => !t.IsAbstract && t.IsSubclassOf(typeof(Controller)))
            //    .Where(t => t.Namespace != null && !t.Name.Contains("T4MVC"))
            //    .Where(c => c.IsDefined(typeof(ClaimsGroupAttribute)))
            //    .Select(c => new ClaimsGroup()
            //    {
            //        GroupName = c.GetCustomAttribute<ClaimsGroupAttribute>().Name,
            //        GroupId = c.FullName,
            //        ControllerType = c,
            //        Claims = GetActionClaims(c),
            //    })
            //    .ToList();

            //return claimedGroups;
        }


        private List<String> GetActionClaims(Type controllerType)
        {
            var result = controllerType.GetMethods()
                .Where(m => m.IsDefined(typeof(ClaimsActionAttribute)))
                .SelectMany(m => m.GetCustomAttributes<ClaimsActionAttribute>())
                .Select(a => a.Claim)
                .Distinct()
                .Select(a => a.ToString())
                .ToList();
                
            return result;
        }

        //public List<String> GetControlledClaims()
        //{
        //    var claimedActions = GetControllerTypes()
        //        .SelectMany(ct => ct.GetMethods())
        //        .Where(m => m.IsDefined(typeof(ClaimsAuthorizeAttribute)))
        //        .Select(m => m.GetCustomAttribute<ClaimsAuthorizeAttribute>())
        //        .Select(a => a.Name)
        //        .ToList();

        //    return claimedActions;
        //}


        public static IEnumerable<Type> GetControllerTypes()
        {
            return Assembly.GetAssembly(typeof(MvcApplication))
                .GetTypes()
                .Where(t => !t.IsAbstract && t.IsSubclassOf(typeof(Controller)))
                .Where(t => t.Namespace != null && !t.Name.Contains("T4MVC"))
                .ToList();
        }
    }

    public class ClaimsGroup
    {
        public ClaimsGroup()
        {
            Claims = new List<string>();
        }

        public String GroupName { get; set; }

        public String GroupId { get; set; }

        public Type ControllerType { get; set; }

        public List<String> Claims { get; set; } 
    }
}