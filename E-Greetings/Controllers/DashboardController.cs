using E_Greetings.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Globalization;

namespace E_Greetings.Controllers
{

    [Authorize(Policy = "View Dashboard")]
    public class DashboardController : Controller
    {
        private readonly EGreetingsContext _db_context;
        public DashboardController(EGreetingsContext db_context)
        {
            _db_context = db_context;
        }

        [Authorize(Policy = "View Dashboard")]
        public IActionResult DashboardIndex()
        {
            var users = _db_context.Users.Count();
            var subscribers = _db_context.Subscribers.Count();
            var cards = _db_context.Cards.Count();
            var feedbacks = _db_context.Feedbacks.Count();
            var cards_sent = _db_context.CardsSent.Count();

            var model = new DashboardViewModel
            {
                Users = users,
                Subscribers = subscribers,
                Cards = cards,
                Feedbacks = feedbacks,
                CardsSent = cards_sent
            };


            var subsCountForChart = _db_context.Subscribers
                     .GroupBy(s => s.CreatedAt.Month)
                     .Select(g => new { Month = g.Key, Count = g.Count() })
                     .ToList();



            var cardsSentForChart = _db_context.CardsSent
              .GroupBy(c => c.CreatedAt.Month)
              .Select(g => new { Month = g.Key, Count = g.Count() })
              .ToList();


            var monthlyData = Enumerable.Range(1, 12).Select(month => new
            {
                MonthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month),
                Subscribers = subsCountForChart.FirstOrDefault(s => s.Month == month)?.Count ?? 0,
                CardsSent = cardsSentForChart.FirstOrDefault(c => c.Month == month)?.Count ?? 0
            }).ToList();


            ViewBag.MonthlyData = JsonConvert.SerializeObject(monthlyData);

            return View(model);
        }


    }
}
