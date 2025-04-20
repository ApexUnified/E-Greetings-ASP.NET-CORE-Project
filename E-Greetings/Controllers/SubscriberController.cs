using E_Greetings.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;

namespace E_Greetings.Controllers
{
    [Authorize(Policy = "View Subscribers")]
    public class SubscriberController : Controller
    {
        private readonly EGreetingsContext _db_context;
        public SubscriberController(EGreetingsContext context)
        {
            _db_context = context;
        }

        [Authorize(Policy = "View Subscribers")]
        public IActionResult Index()
        {
            var subscribers = _db_context.Subscribers
             .GroupJoin(
                 _db_context.Users,          // The Users table
                 s => s.UserId,              // Subscriber's UserId
                 u => u.Id,                  // User's Id
                 (s, users) => new { s, users = users.DefaultIfEmpty() } // Perform LEFT JOIN
             )
             .Select(x => new SubscriberViewModel
             {
                 Id = x.s.Id,
                 Name = x.s.UserId != null ? x.users.FirstOrDefault().Name : "GUEST",
                 Fee = x.s.Fee,
                 SubscriptionEndDate = x.s.SubscriptionEndDate,
                 SubscriptionStartDate = x.s.SubscriptionStartDate,
                 Status = x.s.Status,
                 Email = x.s.Email,
                 IsGuest = x.s.IsGuest,
                 IpAddress = x.s.IpAddress
             }).ToList();



            //var sqlQuery = subscribers.ToQueryString();


            //return Json(new { subscribers });
            return View(subscribers);
        }



        [Authorize(Policy = "Edit Subscribers")]
        public async Task<IActionResult> Active(int id)
        {
            if(id == null)
            {
                TempData["Error"] = "Id Is Missing Of Subscription";
                return RedirectToAction("Index", "Subscriber");
            }

            var subscriber = await _db_context.Subscribers
            .Where(s => s.Id == id && s.IsGuest == 0)
            .FirstOrDefaultAsync();



            if (subscriber != null)
            {
                subscriber.Status = 0;
            }

            if (subscriber == null)
            {
                var guest = new Subscriber
                {
                    Id = id,
                    Status = 1
                };

                _db_context.Subscribers.Attach(guest);
                _db_context.Entry(guest).Property(x => x.Status).IsModified = true;
                await _db_context.SaveChangesAsync();
                TempData["Success"] = "Subscription Status Has Been Changes";
                return RedirectToAction("Index", "Subscriber");
            }

            subscriber.Status = 1;
            await _db_context.SaveChangesAsync();

            TempData["Success"] = "Subscription Status Has Been Changes";
            return RedirectToAction("Index", "Subscriber");

        }


        [Authorize(Policy = "Edit Subscribers")]
        public async Task<IActionResult> InActive(int id)
        {
            if (id == null)
            {
                TempData["Error"] = "Id Is Missing Of Subscription";
                return RedirectToAction("Index", "Subscriber");
            }

            var subscriber = await _db_context.Subscribers
            .Where(s => s.Id == id && s.IsGuest == 0)
            .FirstOrDefaultAsync();

          

            if (subscriber != null)
            {
                subscriber.Status = 0;
            }

           if(subscriber == null)
            {
                var guest = new Subscriber
                {
                    Id = id,
                    Status = 0
                };

                _db_context.Subscribers.Attach(guest);
                _db_context.Entry(guest).Property(x => x.Status).IsModified = true;
                await _db_context.SaveChangesAsync();
                TempData["Success"] = "Subscription Status Has Been Changes";
                return RedirectToAction("Index", "Subscriber");
            }

           subscriber.Status = 0;
           await _db_context.SaveChangesAsync();

            TempData["Success"] = "Subscription Status Has Been Changes";
            return RedirectToAction("Index", "Subscriber");

        }

        [Authorize(Policy = "Delete Subscribers")]
        public async Task<IActionResult> destroy(int id)
        {
            if (id == null)
            {
                TempData["Error"] = "Id Is Missing Of Subscription";
                return RedirectToAction("Index", "Subscriber");
            }

            var subscriber = await _db_context.Subscribers
           .Where(s => s.Id == id && s.IsGuest == 0)
           .FirstOrDefaultAsync();



            if (subscriber != null)
            {
                _db_context.Subscribers.Remove(subscriber);
                await _db_context.SaveChangesAsync();
                TempData["Success"] = "Subscriber Has Been Deleted";
                return RedirectToAction("Index", "Subscriber");
            }

           
                var guest = new Subscriber
                {
                    Id = id,
                };

                _db_context.Subscribers.Remove(guest);
                _db_context.Entry(guest).Property(x => x.Status).IsModified = true;
                await _db_context.SaveChangesAsync();
                TempData["Success"] = "Subscriber Has Been Deleted";
                return RedirectToAction("Index", "Subscriber");
            

          
        }

        [Authorize(Policy = "Delete Subscribers")]
        [HttpPost]
        [Route("subscriber/deletebyselection")]
        public IActionResult DestroyBySelection(int[] subscriber_ids)
        {
            if(subscriber_ids == null)
            {
                return Json(new { status = false });
            }

            var subscribers = _db_context.Subscribers.Where(x => subscriber_ids.Contains(x.Id) && x.IsGuest == 0).ToList();
                if(subscribers != null || subscribers.Any())
                {
                    _db_context.Subscribers.RemoveRange(subscribers);
                    _db_context.SaveChanges();

                }

                var guests = _db_context.Subscribers
                    .Where(x => subscriber_ids.Contains(x.Id) && x.IsGuest == 1)
                    .Select(x => new
                    {
                        x.Id,
                    })
                    .ToList();

                if (guests.Any())
                {
                    foreach (var guest in guests)
                    {
                        var guestSubscriber = new Subscriber { Id = guest.Id };
                        _db_context.Subscribers.Remove(guestSubscriber);
                    }

                     _db_context.SaveChangesAsync();
                }


            return Json(new { status = true });

        }
    }


}
