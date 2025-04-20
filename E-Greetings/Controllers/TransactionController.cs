using E_Greetings.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_Greetings.Controllers
{

    [Authorize(Policy = "View Reports")]
    public class TransactionController : Controller
    {

        private readonly EGreetingsContext _db_context;

        public TransactionController(EGreetingsContext db_context)
        {
            _db_context = db_context;
        }

        [Authorize(Policy = "View Reports")]
        public IActionResult Index()
        {
            var transactions = _db_context.Transactions.ToList();
            return View(transactions);
        }
    }
}
