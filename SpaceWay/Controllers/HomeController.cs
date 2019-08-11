using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SpaceWay.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            //send the username which is logged in to the index
            ViewBag.LoggedInUser = Session["Username"];
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        //Nasa web service
        public ActionResult Nasa()
        {
            return View();
        }

        // Graphs statistics
        public ActionResult Statistics()
        {
            return View();
        }
    }
}