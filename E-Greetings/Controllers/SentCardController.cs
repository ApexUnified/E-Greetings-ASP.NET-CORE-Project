using E_Greetings.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_Greetings.Controllers
{

    [Authorize(Policy = "View Reports")]
    public class SentCardController : Controller
    {

        private readonly EGreetingsContext _db_context;

        public SentCardController(EGreetingsContext db_context)
        {
            _db_context = db_context;
        }

        [Authorize(Policy = "View Reports")]
        public IActionResult Index()
        {
            var sentCards = _db_context.CardsSent
            .Include(x => x.Card) 
            .Select(x=> new CardSentViewModel
            {
               Id = x.Id,
               User = x.User_Name,
               Card = x.Card.CardName,
               SentFrom = x.SentFrom,
               SentTo =  x.SentTo,
               SentAt = x.SentAt
            })
            .ToList();
            ViewData["sentCards"] = sentCards;

            //return Json(new { sentCards });
            return View();
        }
    }
}
