using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;
using Microsoft.Data.Entity;
using FacebookAutomation.Models;

namespace FacebookAutomation.Controllers
{
    [Produces("application/json")]
    [Route("api/Produits")]
    public class ProduitsController : Controller
    {
        private ApplicationDbContext _context;

        public ProduitsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Produits
        [HttpGet]
        public IEnumerable<Produits> GetProduits()
        {
            return _context.Produits;
        }

        // GET: api/Produits/5
        [HttpGet("{id}", Name = "GetProduits")]
        public IActionResult GetProduits([FromRoute] string id)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }

            Produits produits = _context.Produits.Single(m => m.Id == id);

            if (produits == null)
            {
                return HttpNotFound();
            }

            return Ok(produits);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ProduitsExists(string id)
        {
            return _context.Produits.Count(e => e.Id == id) > 0;
        }
    }
}