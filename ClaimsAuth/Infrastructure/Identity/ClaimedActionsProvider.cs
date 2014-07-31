using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using ClaimsAuth.Infrastructure.Helpers;


namespace ClaimsAuth.Infrastructure.Identity
{
    public class ClaimedActionsProvider
    {
        public List<ClaimsGroup> GetControlledClaims()
        {
            var claimedGroups = Assembly.GetAssembly(typeof(MvcApplication))
                .GetTypes()
                .Where(t => !t.IsAbstract && t.IsSubclassOf(typeof(Controller)))
                .Where(t => t.Namespace != null && !t.Name.Contains("T4MVC"))
                .Where(c => c.IsDefined(typeof(ClaimsGroupAttribute)))
                .Select(c => new ClaimsGroup()
                {
                    GroupName = GetGroupName(c),
                    GroupId = GetGroupId(c),
                    ControllerType = c,
                    Claims = GetActionClaims(c),
                })
                .ToList();

            return claimedGroups;
        }


        private String GetGroupName(Type controllerType)
        {
            var result = controllerType.GetCustomAttribute<ClaimsGroupAttribute>().Resource.GetDisplayName();
            return result;
        }


        private int GetGroupId(Type controllerType)
        {
            var claimsGroupAttribute = controllerType.GetCustomAttribute<ClaimsGroupAttribute>();
            var result = (int)claimsGroupAttribute.Resource;
            return result;
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
    }


    public class ClaimsGroup
    {
        public ClaimsGroup()
        {
            Claims = new List<string>();
        }

        public String GroupName { get; set; }

        public int GroupId { get; set; }

        public Type ControllerType { get; set; }

        public List<String> Claims { get; set; } 
    }
}