using E_Greetings.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_Greetings.Controllers
{



    [Authorize(Policy = "View Cards")]
    public class CardController : Controller
    {

        private readonly EGreetingsContext _db_context;

        public CardController(EGreetingsContext db_context)
        {
            _db_context = db_context;
        }

        [Authorize(Policy = "View Cards")]
        public IActionResult Index()
        {
            var cards = _db_context.Cards
                .Include(u => u.CardLists)
                .Select(x => new CardViewModel
                {
                    Id = x.Id,
                    CardName = x.CardLists != null ? x.CardLists.CardName : null,
                    CardController = x.CardLists != null ? x.CardLists.CardController : null,
                    CardAction = x.CardLists != null ? x.CardLists.CardAction : null,
                    Status = x.Status
                }).ToList();


            //return Json(new { cards });


            return View(cards);
        }

        [Authorize(Policy = "Create Cards")]
        public IActionResult Create()
        {

            var cards = _db_context.Cards.ToList();
            var cards_list = _db_context.CardsList.ToList();

            if(cards.Count == cards_list.Count)
            {
                TempData["Error"] = "You Cant Create More Cards You Have Already Created It All";
                return RedirectToAction("Index", "Card");
            }

            var cardIds = cards.Select(c => c.Card_id).ToList();
            ViewData["cards_list"] = cards_list;
            ViewData["cardIds"] = cardIds;

            return View();
        }

        [Authorize(Policy = "Create Cards")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(List<int> card_ids , int? status) 
        {

            if (card_ids == null || !card_ids.Any())
            {
                var cards_lists = _db_context.CardsList.ToList();
                ViewData["cards_list"] = cards_lists;
                TempData["Error"] = "Please select at least one card.";
                return RedirectToAction("Create", "Card");
            }

            if (status == null)
            {
                var cards_listss = _db_context.CardsList.ToList();
                ViewData["cards_list"] = cards_listss;
                TempData["Error"] = "Card Status Is Missing";
                return RedirectToAction("Create", "Card");
            }

            if (ModelState.IsValid)
            {
                //return Json(new { card_ids });

                var cards = new List<Card>();

                foreach (var item in card_ids)
                {
                    cards.Add(new Card
                    {
                        Card_id = item,
                        Status = Convert.ToInt32(status),
                    });
                }

                _db_context.Cards.AddRange(cards);
                _db_context.SaveChanges();
                TempData["Success"] = "Card Has Been Added Succesfully";
                return RedirectToAction("Index", "Card");
            }


            var cards_listsss = _db_context.CardsList.ToList();
            ViewData["cards_list"] = cards_listsss;
            return View();
        }


        [Authorize(Policy = "Edit Cards")]
        public IActionResult Active(int? id)
        {
            if (id == null)
            {
                TempData["Error"] = "Id Is Missing Of Card And Cannot Be Updated";
                return RedirectToAction("Index", "Card");
            }

            var card = _db_context.Cards.FirstOrDefault(x => x.Id == id);
            if(card == null)
            {
                TempData["Error"] = "Card Not Found To Update The Status";
                return RedirectToAction("Index", "Card");
            }

            card.Status = 1;

            _db_context.Cards.Update(card);
            _db_context.SaveChanges();
            TempData["Success"] = "Card Status Has Been Saved";
            return RedirectToAction("Index", "Card");

        }


        [Authorize(Policy = "Edit Cards")]
        public IActionResult InActive(int? id)
        {
            if (id == null)
            {
                TempData["Error"] = "Id Is Missing Of Card And Cannot Be Updated";
                return RedirectToAction("Index", "Card");
            }

            var card = _db_context.Cards.FirstOrDefault(x => x.Id == id);
            if (card == null)
            {
                TempData["Error"] = "Card Not Found To Update The Status";
                return RedirectToAction("Index", "Card");
            }

            card.Status = 0;

            _db_context.Cards.Update(card);
            _db_context.SaveChanges();
            TempData["Success"] = "Card Status Has Been Saved";
            return RedirectToAction("Index", "Card");

        }


        [Authorize(Policy = "Delete Cards")]
        public IActionResult Destroy(int? id)
        {
            if (id == null)
            {
                TempData["Error"] = "Id Is Missing Of Card And Cannot Delete";
                return RedirectToAction("Index", "Card");
            }

            var card = _db_context.Cards.FirstOrDefault(x => x.Id == id);

            if(card == null)
            {
                TempData["Error"] = "Card  is Missing And Cannot Delete";
                return RedirectToAction("Index", "Card");
            }


            _db_context.Cards.Remove(card);
            _db_context.SaveChanges();
            TempData["Success"] = "Card Has Been Deleted Succesfully";
            return RedirectToAction("Index", "Card");
        }



        [Authorize(Policy = "Delete Cards")]
        [HttpPost]
        [Route("card/deletebyselection")]

        public IActionResult DestroyBySelection(int[] card_ids) 
        {
            if(card_ids == null)
            {
                return Json(new { status = false });
            }

            var cards = _db_context.Cards.Where(x=> card_ids.Contains(x.Id)).ToList();
            if(cards == null || !cards.Any())
            {
                return Json(new { status = false });
            }

            _db_context.Cards.RemoveRange(cards);
            _db_context.SaveChanges();
            return Json(new { status = true });
        }
    }
}
