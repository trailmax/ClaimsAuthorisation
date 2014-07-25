using System;
using System.Web.Mvc;


namespace ClaimsAuth.Areas.Product.Controllers
{
    public class ProductTypeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(String id)
        {
            ViewBag.Id = id;
            return View();
        }

           
        public ActionResult Edit()
        {
            return View();
        }


        [HttpPost]
        public ActionResult Edit(String id)
        {
            ViewBag.Id = id;
            return View();
        }
    }
}