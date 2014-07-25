//using System;
//using System.Security.Claims;
//using System.Web;
//using System.Web.Mvc;
//using System.Web.Routing;


//namespace ClaimsAuth.Infrastructure.Identity
//{
//    public class ClaimsAuthorizeAttribute : AuthorizeAttribute
//    {
//        public string Name { get; private set; }


//        public ClaimsAuthorizeAttribute(string name)
//        {
//            Name = name;
//        }

//        public override void OnAuthorization(AuthorizationContext filterContext)
//        {
//            var user = HttpContext.Current.User as ClaimsPrincipal;
//            if (user.HasClaim(Name, Name))
//            {
//                base.OnAuthorization(filterContext);
//            }
//            else
//            {
//                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary()
//                {
//                    {"controller", "errors"},
//                    {"action", "Unauthorised"}
//                });
//            }
//        }
//    }
//}