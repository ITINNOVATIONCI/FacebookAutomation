using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;
using Microsoft.Data.Entity;
using FacebookAutomation.Models;

namespace FacebookAutomation.Controllers
{
    [Produces("application/json")]
    [Route("api/Licences")]
    public class LicencesController : Controller
    {
        private ApplicationDbContext _context;

        public LicencesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Licences
        [HttpGet]
        public IEnumerable<Licences> GetLicences()
        {
            return _context.Licences;
        }

        // GET: api/Licences/5
        [HttpGet("{id}", Name = "GetLicences")]
        public IActionResult GetLicences([FromRoute] string id)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }

            Licences licences = _context.Licences.Single(m => m.Id == id);

            if (licences == null)
            {
                return HttpNotFound();
            }

            return Ok(licences);
        }

        // PUT: api/Licences/5
        [HttpPut("{id}")]
        public IActionResult PutLicences(string id, [FromBody] Licences licences)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }

            if (id != licences.Id)
            {
                return HttpBadRequest();
            }

            _context.Entry(licences).State = EntityState.Modified;

            try
            {
                _context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LicencesExists(id))
                {
                    return HttpNotFound();
                }
                else
                {
                    throw;
                }
            }

            return new HttpStatusCodeResult(StatusCodes.Status204NoContent);
        }

        // POST: api/Licences
        [HttpPost]
        public IActionResult PostLicences([FromBody] Licences licences)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }

            _context.Licences.Add(licences);
            try
            {
                _context.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (LicencesExists(licences.Id))
                {
                    return new HttpStatusCodeResult(StatusCodes.Status409Conflict);
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("GetLicences", new { id = licences.Id }, licences);
        }

        // DELETE: api/Licences/5
        [HttpDelete("{id}")]
        public IActionResult DeleteLicences(string id)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }

            Licences licences = _context.Licences.Single(m => m.Id == id);
            if (licences == null)
            {
                return HttpNotFound();
            }

            _context.Licences.Remove(licences);
            _context.SaveChanges();

            return Ok(licences);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool LicencesExists(string id)
        {
            return _context.Licences.Count(e => e.Id == id) > 0;
        }
    }
}