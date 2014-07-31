using System;
using System.Web.Mvc;
using ClaimsAuth.Infrastructure.Identity;


namespace ClaimsAuth.Areas.Product.Controllers
{
    [ClaimsGroup(ClaimResources.Products)]
    public class ProductController : Controller
    {
        [ClaimsAction(ClaimsActions.Index)]
        public ActionResult Index()
        {
            return View();
        }


        [ClaimsAction(ClaimsActions.Create)]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ClaimsAction(ClaimsActions.Create)]
        public ActionResult Create(String id)
        {
            ViewBag.Id = id;
            return View();
        }


        [ClaimsAction(ClaimsActions.View)]
        public ActionResult Edit()
        {
            return View();
        }


        [HttpPost]
        [ClaimsAction(ClaimsActions.Edit)]
        public ActionResult Edit(String id)
        {
            ViewBag.Id = id;
            return View();
        }
    }
}