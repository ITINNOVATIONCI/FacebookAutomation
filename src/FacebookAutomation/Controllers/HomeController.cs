using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using FacebookAutomation.Models;
using Microsoft.ApplicationInsights;

namespace FacebookAutomation.Controllers
{
    public class HomeController : Controller
    {
        private TelemetryClient telemetry = new TelemetryClient();
        protected string URISignature = "http://api.sandbox.cinetpay.com/v1/?method=getSignatureByPost";
        protected string URIStatus = "http://api.sandbox.cinetpay.com/v1/?method=checkPayStatus";

        private ApplicationDbContext _dbContext;
        public string currentUserId { get; set; }
        public ApplicationUser currentUser { get; set; }
        public string Name { get; set; }
        string signature;
        public string message { get; set; }

        public HomeController(ApplicationDbContext dbContext, TelemetryClient Telemetry)
        {
            _dbContext = dbContext;
            telemetry = Telemetry;

        }

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
            Transactions trans = new Transactions();
            trans.idProduit = id;
            trans.produit = _dbContext.Produits.Where(c => c.Id == id).FirstOrDefault();

            return View(trans);
        }

        public IActionResult Paiement(Transactions trans)
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
