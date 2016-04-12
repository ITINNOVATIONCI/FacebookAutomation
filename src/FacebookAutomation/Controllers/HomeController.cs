using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;

namespace FacebookAutomation.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Faq()
        {

            return View();
        }

        public IActionResult Feature()
        {

            return View();
        }

        public IActionResult Galerie()
        {

            return View();
        }

        public IActionResult Pricing()
        {

            return View();
        }

        public IActionResult Shopcart(string id)
        {

            return View();
        }

        public IActionResult Contact()
        {

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
