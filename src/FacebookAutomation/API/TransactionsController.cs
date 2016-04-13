using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;
using Microsoft.Data.Entity;
using FacebookAutomation.Models;

namespace FacebookAutomation.Controllers
{
    [Produces("application/json")]
    [Route("api/Transactions")]
    public class TransactionsController : Controller
    {
        private ApplicationDbContext _context;

        public TransactionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Transactions
        [HttpGet]
        public IEnumerable<Transactions> GetTransactions()
        {
            return _context.Transactions;
        }

        // GET: api/Transactions/5
        [HttpGet("{id}", Name = "GetTransactions")]
        public IActionResult GetTransactions([FromRoute] string id)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }

            Transactions transactions = _context.Transactions.Single(m => m.Id == id);

            if (transactions == null)
            {
                return HttpNotFound();
            }

            return Ok(transactions);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool TransactionsExists(string id)
        {
            return _context.Transactions.Count(e => e.Id == id) > 0;
        }
    }
}