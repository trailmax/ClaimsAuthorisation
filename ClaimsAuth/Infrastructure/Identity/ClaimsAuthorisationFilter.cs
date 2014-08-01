using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;


namespace ClaimsAuth.Infrastructure.Identity
{
    public class ClaimsAuthorisationFilter : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            var user = HttpContext.Current.User as ClaimsPrincipal;
            var controllerGroup = filterContext.Controller.GetType().GetCustomAttribute<ClaimsGroupAttribute>();

            if (controllerGroup == null)
            {
                base.OnAuthorization(filterContext);
                return;
            }

            var actionClaim = filterContext.ActionDescriptor.GetCustomAttributes(typeof(ClaimsActionAttribute), true).FirstOrDefault();

            actionClaim = actionClaim ?? new ClaimsActionAttribute(ClaimsActions.Index);

            if (user.HasClaim(controllerGroup.GetGroupId(), actionClaim.ToString()))
            {
                base.OnAuthorization(filterContext);
            }
            else
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary()
                {
                    {"controller", "errors"},
                    {"action", "Unauthorised"}
                });
            }
        }
    }
}