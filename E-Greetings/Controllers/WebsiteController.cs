using E_Greetings.Models;
using E_Greetings.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;
using System.Net;
using Humanizer;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;

namespace E_Greetings.Controllers
{
    public class WebsiteController : Controller
    {


        private readonly EGreetingsContext _db_context;
        private readonly MailService _emailService;

        public WebsiteController(EGreetingsContext db_context, MailService emailService)
        {
            _db_context = db_context;
            _emailService = emailService;
        }

        [Route("/")]
        public IActionResult Home()
        {
            return View();
        }

        [Route("About")]
        public IActionResult About()
        {
            var users = _db_context.Users.Count();
            var cards = _db_context.Cards.Count();
            var sentcards = _db_context.CardsSent.Count();
            ViewData["users"] = users;
            ViewData["cards"] = cards;
            ViewData["sentcards"] = sentcards;
            return View();
        }

        [Route("Contact")]
        public IActionResult Contact()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ContactUs(string? name, string? email, string? message)
        {

            if(string.IsNullOrEmpty(name) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(message))
            {
                TempData["Error"] = "Please Fill All Fields";
                return RedirectToAction("Contact", "Website");
            }

            var users = _db_context.Users.Select(r=> new
            {
                Role = r.Role.Name,
                r.Name,
                r.Email
            }).Where(x=> x.Role == "Admin").ToList();

            var subject = "Contact Us Email From " + name;
            foreach(var user in users)
            {
                _emailService.SendEmail(
                   to: user.Email,
                   subject: subject,
                   body: "<h2> From: "+ name +"</h2> " + "<br/>" + "<h5> Email: "+ email + "</h5> "  +   "<br/>"  + "<p> " + message + "</p>"
               );
            }
            TempData["Success"] = "Your Message Has Been Sent To Our Admin";
            return RedirectToAction("Contact", "Website");
        }


        [Authorize]
        [Route("Feedback")]
        public IActionResult Feedback()
        {
            var feedbacks = _db_context.Feedbacks.Where(x=> x.Status == 1).ToList();
            ViewData["feedbacks"] = feedbacks;

            return View();
        }

        [Route("Feedback")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Feedback(Feedback feedback)
        {
            if (ModelState.IsValid)
            {
                feedback.Status = 0;
                _db_context.Feedbacks.Add(feedback);
                _db_context.SaveChanges();
                TempData["Success"] = "Feedback Has Been Created Succesfuly";
                return RedirectToAction("Feedback", "Website");
            }
            var feedbacks = _db_context.Feedbacks.Where(x => x.Status == 1).ToList();
            ViewData["feedbacks"] = feedbacks;
            TempData["Error"] = "Error Ocucred While Submiting Your Feedback ";
            return View(feedback);
        }






   
        [Route("Wedding")]
        [HttpPost]
        public IActionResult Wedding(int? id)
        {


            if (User.Identity.IsAuthenticated)
            {
                var claimsIdentity = User.Identity as ClaimsIdentity;
                var userEmail = claimsIdentity?.FindFirst(ClaimTypes.Email)?.Value;

                var user = _db_context.Users.Where(x => x.Email == userEmail).FirstOrDefault();

                var is_subscribed = _db_context.Subscribers.FirstOrDefault(x => x.UserId == user.Id && x.Status == 1 && x.SubscriptionEndDate >= DateTime.Now);
                if (is_subscribed == null)
                {
                    TempData["Error"] = "Please Subscribe First Subscription is Required For Sending Cards";
                    return RedirectToAction("Subscription", "Website");
                }
            }
            else
            {
                var ipAddress = HttpContext.Connection.RemoteIpAddress;
                var formattedIpAddress = ipAddress.IsIPv4MappedToIPv6
                    ? ipAddress.MapToIPv4().ToString()
                    : ipAddress.ToString();
                var is_subscribed = _db_context.Subscribers.Where(x => x.IpAddress == formattedIpAddress && x.Status == 1 && x.SubscriptionEndDate >= DateTime.Now).Select(e => new
                {
                    e.Id,
                    e.Status,
                    e.IsGuest,
                    e.Email,
                    e.IpAddress
                }).FirstOrDefault();
                if (is_subscribed == null)
                {
                    TempData["Error"] = "Please Subscribe First Subscription is Required For Sending Cards";
                    return RedirectToAction("Subscription", "Website");
                }
            }


            if (id == null)
            {
                TempData["Error"] = "Id Cannot Be Null Of Card Its Must Required";
                return RedirectToAction("Home", "Website");
            }

            ViewData["card_id"] = id;

            var cardexits = _db_context.CardsList.FirstOrDefault(x => x.Id == id);
            if (cardexits == null)
            {
                TempData["Error"] = "Card Not Found Please Try Again Later";
                return RedirectToAction("Home", "Website");
            }

            var card = _db_context.Cards.FirstOrDefault(x => x.Card_id == id);
            if (card == null)
            {
                TempData["Error"] = "Card Not Found Please Try Again Later";
                return RedirectToAction("Home", "Website");
            }

            if (card.Status == 1)
            {
                return View();
            }
            else
            {
                TempData["Error"] = "The Card You Are Trying TO Access Its Not Activated Yet";
                return RedirectToAction("Home", "Index");
            }


        }

     
        [Route("Wedding/SendEmail")]
        [HttpPost]
        public IActionResult WeddingSendEmail(string cardHtml, string[] emails, int card_id)
        {

            if (string.IsNullOrEmpty(cardHtml) || emails == null || emails.Length == 0)
            {
                return Json( new {status = false, message= "Card HTML is empty or no emails provided." });
            }


            var subject = "Wedding Invitation";
            foreach (var email in emails)
            {
                _emailService.SendEmail(
                    to: email,
                    subject: subject,
                    body: cardHtml
                );
            }


            if (User.Identity.IsAuthenticated)
            {
                var claimsIdentity = User.Identity as ClaimsIdentity;
                var userEmail = claimsIdentity?.FindFirst(ClaimTypes.Email)?.Value;

                var user = _db_context.Users.Where(x => x.Email == userEmail).FirstOrDefault();
                string sentFrom = user.Name != null ? user.Name : "Guest";
                DateTime nowDate = DateTime.Now.Date;
                var sentRecords = emails.Select(email => new CardSent
                {
                    User_Name = user.Name,
                    SentFrom = sentFrom,
                    SentTo = email,
                    Card_Id = card_id,
                    SentAt = nowDate,
                    CreatedAt = DateTime.Now
                }).ToList();
                _db_context.CardsSent.AddRange(sentRecords);
                _db_context.SaveChanges();
            }
            else
            {
                DateTime nowDate = DateTime.Now.Date;
                string sentFrom =  "Guest";
                var sentRecords = emails.Select(email => new CardSent
                {
                    User_Name = "Guest",
                    SentFrom = sentFrom,
                    SentTo = email,
                    Card_Id = card_id,
                    SentAt = nowDate,
                    CreatedAt = DateTime.Now
                }).ToList();

                _db_context.CardsSent.AddRange(sentRecords);
                _db_context.SaveChanges();
            }


            return Json(new { status = true, message = "Card Has Been Sent Succesfully" });
                
        }



  
        [Route("Anyversary")]
        [HttpPost]
        public IActionResult Anyversary(int? id)
        {

            if (User.Identity.IsAuthenticated)
            {
                var claimsIdentity = User.Identity as ClaimsIdentity;
                var userEmail = claimsIdentity?.FindFirst(ClaimTypes.Email)?.Value;

                var user = _db_context.Users.Where(x => x.Email == userEmail).FirstOrDefault();

                var is_subscribed = _db_context.Subscribers.FirstOrDefault(x => x.UserId == user.Id && x.Status == 1 && x.SubscriptionEndDate >= DateTime.Now);
                if (is_subscribed == null)
                {
                    TempData["Error"] = "Please Subscribe First Subscription is Required For Sending Cards";
                    return RedirectToAction("Subscription", "Website");
                }
            }
            else
            {
                var ipAddress = HttpContext.Connection.RemoteIpAddress;
                var formattedIpAddress = ipAddress.IsIPv4MappedToIPv6
                    ? ipAddress.MapToIPv4().ToString()
                    : ipAddress.ToString();

                var is_subscribed = _db_context.Subscribers.Where(x => x.IpAddress == formattedIpAddress && x.Status == 1 && x.SubscriptionEndDate >= DateTime.Now).Select(e => new
                {
                    e.Id,
                    e.Status,
                    e.IsGuest,
                    e.Email,
                    e.IpAddress
                }).FirstOrDefault();
                if (is_subscribed == null)
                {
                    TempData["Error"] = "Please Subscribe First Subscription is Required For Sending Cards";
                    return RedirectToAction("Subscription", "Website");
                }
            }
           

            if (id == null)
            {
                TempData["Error"] = "Id Cannot Be Null Of Card Its Must Required";
                return RedirectToAction("Home", "Website");
            }

            ViewData["card_id"] = id;

            var cardexits = _db_context.CardsList.FirstOrDefault(x => x.Id == id);
            if (cardexits == null)
            {
                TempData["Error"] = "Card Not Found Please Try Again Later";
                return RedirectToAction("Home", "Website");
            }

            var card = _db_context.Cards.FirstOrDefault(x => x.Card_id == id);
            if (card == null)
            {
                TempData["Error"] = "Card Not Found Please Try Again Later";
                return RedirectToAction("Home", "Website");
            }

            if (card.Status == 1)
            {
                return View();
            }
            else
            {
                TempData["Error"] = "The Card You Are Trying TO Access Its Not Activated Yet";
                return RedirectToAction("Home", "Index");
            }
        }



        [Route("Anyversary/SendEmail")]
        [HttpPost]
        public IActionResult AnyversarySendEmail(string cardHtml, string[] emails, int card_id)
        {

            

            // Validate input
            if (string.IsNullOrEmpty(cardHtml) || emails == null || emails.Length == 0)
            {
                return Json(new { status = false, message = "Error Occured While Sending Card" });
            }


            var subject = "Anyversary Invitation";
            foreach (var email in emails)
            {
                _emailService.SendEmail(
                    to: email,
                    subject: subject,
                    body: cardHtml
                );
            }


            if (User.Identity.IsAuthenticated)
            {
                var claimsIdentity = User.Identity as ClaimsIdentity;
                var userEmail = claimsIdentity?.FindFirst(ClaimTypes.Email)?.Value;

                var user = _db_context.Users.Where(x => x.Email == userEmail).FirstOrDefault();

                string sentFrom = user.Name != null ? user.Name : "Guest";
                DateTime nowDate = DateTime.Now.Date;
                var sentRecords = emails.Select(email => new CardSent
                {
                    User_Name = user.Name,
                    SentFrom = sentFrom,
                    SentTo = email,
                    Card_Id = card_id,
                    SentAt = nowDate,
                    CreatedAt = DateTime.Now
                }).ToList();

                _db_context.CardsSent.AddRange(sentRecords);
                _db_context.SaveChanges();
            }else
            {
                DateTime nowDate = DateTime.Now.Date;
                string sentFrom = "Guest";
                var sentRecords = emails.Select(email => new CardSent
                {
                    User_Name = "Guest",
                    SentFrom = sentFrom,
                    SentTo = email,
                    Card_Id = card_id,
                    SentAt = nowDate,
                    CreatedAt = DateTime.Now
                }).ToList();

                _db_context.CardsSent.AddRange(sentRecords);
                _db_context.SaveChanges();
            }


            return Json(new { status = true, message = "Card Has Been Sent Succesfully" });
        }


        [Route("Birthday")]
        [HttpPost]
        public IActionResult Birthday(int? id)
        {


            if (User.Identity.IsAuthenticated)
            {
                var claimsIdentity = User.Identity as ClaimsIdentity;
                var userEmail = claimsIdentity?.FindFirst(ClaimTypes.Email)?.Value;

                var user = _db_context.Users.Where(x => x.Email == userEmail).FirstOrDefault();

                var is_subscribed = _db_context.Subscribers.FirstOrDefault(x => x.UserId == user.Id && x.Status == 1 && x.SubscriptionEndDate >= DateTime.Now);
                if (is_subscribed == null)
                {
                    TempData["Error"] = "Please Subscribe First Subscription is Required For Sending Cards";
                    return RedirectToAction("Subscription", "Website");
                }
            }
            else
            {
                var ipAddress = HttpContext.Connection.RemoteIpAddress;
                var formattedIpAddress = ipAddress.IsIPv4MappedToIPv6
                    ? ipAddress.MapToIPv4().ToString()
                    : ipAddress.ToString();
                var is_subscribed = _db_context.Subscribers.Where(x => x.IpAddress == formattedIpAddress && x.Status == 1 && x.SubscriptionEndDate >= DateTime.Now).Select(e => new
                {
                    e.Id,
                    e.Status,
                    e.IsGuest,
                    e.Email,
                    e.IpAddress
                }).FirstOrDefault();
                if (is_subscribed == null)
                {
                    TempData["Error"] = "Please Subscribe First Subscription is Required For Sending Cards";
                    return RedirectToAction("Subscription", "Website");
                }
            }

            if (id == null)
            {
                TempData["Error"] = "Id Cannot Be Null Of Card Its Must Required";
                return RedirectToAction("Home", "Website");
            }

            ViewData["card_id"] = id;

            var cardexits = _db_context.CardsList.FirstOrDefault(x => x.Id == id);
            if (cardexits == null)
            {
                TempData["Error"] = "Card Not Found Please Try Again Later";
                return RedirectToAction("Home", "Website");
            }

            var card = _db_context.Cards.FirstOrDefault(x => x.Card_id == id);
            if (card == null)
            {
                TempData["Error"] = "Card Not Found Please Try Again Later";
                return RedirectToAction("Home", "Website");
            }

            if (card.Status == 1)
            {
                return View();
            }
            else
            {
                TempData["Error"] = "The Card You Are Trying TO Access Its Not Activated Yet";
                return RedirectToAction("Home", "Index");
            }
        }



        [Route("Birthday/SendEmail")]
        [HttpPost]
        public IActionResult BirthdaySendEmail(string cardHtml, string[] emails, int card_id)
        {
            
            // Validate input
            if (string.IsNullOrEmpty(cardHtml) || emails == null || emails.Length == 0)
            {
                return Json(new { status = true, message = "Error Occured While Sending Email" });
            }


            var subject = "Birthday Invitation";
            foreach (var email in emails)
            {
                _emailService.SendEmail(
                    to: email,
                    subject: subject,
                    body: cardHtml
                );
            }



            if (User.Identity.IsAuthenticated)
            {
                var claimsIdentity = User.Identity as ClaimsIdentity;
                var userEmail = claimsIdentity?.FindFirst(ClaimTypes.Email)?.Value;

                var user = _db_context.Users.Where(x => x.Email == userEmail).FirstOrDefault();

                string sentFrom = user.Name != null ? user.Name : "Guest";
                DateTime nowDate = DateTime.Now.Date;
                var sentRecords = emails.Select(email => new CardSent
                {
                    User_Name = user.Name,
                    SentFrom = sentFrom,
                    SentTo = email,
                    Card_Id = card_id,
                    SentAt = nowDate,
                    CreatedAt = DateTime.Now
                }).ToList();

                _db_context.CardsSent.AddRange(sentRecords);
                _db_context.SaveChanges();
            } else
            {
                
                DateTime nowDate = DateTime.Now.Date;
                string sentFrom = "Guest";
                var sentRecords = emails.Select(email => new CardSent
                {
                    User_Name = "Guest",
                    SentFrom = sentFrom,
                    SentTo = email,
                    Card_Id = card_id,
                    SentAt = nowDate,
                    CreatedAt = DateTime.Now
                }).ToList();

                _db_context.CardsSent.AddRange(sentRecords);
                _db_context.SaveChanges();
                
            }


            return Json(new { status = true, message = "Card Has Been Sent Succesfully" });
        }


        [Route("Subscription")]
        public IActionResult Subscription()
        {
            return View();
        }


        [Route("Subscription/save")]
        [HttpPost]
        public IActionResult SubscriptionSave(string? email)
        {

            if (User.Identity.IsAuthenticated)
            {
                var claimsIdentity = User.Identity as ClaimsIdentity;
                var userEmail = claimsIdentity?.FindFirst(ClaimTypes.Email)?.Value;

                var user = _db_context.Users.Where(x => x.Email == userEmail).FirstOrDefault();
                if (user == null)
                {
                    return Json(new { status = false });
                }

                DateTime currentDate = DateTime.Now.Date;
                DateTime after30Days = currentDate.AddDays(30);

                var checkingAlreadySubscribed = _db_context.Subscribers
                    .Where(u => u.UserId == user.Id &&
                    u.SubscriptionEndDate > DateTime.Now).FirstOrDefault();

                if (checkingAlreadySubscribed != null)
                {
                    return Json(new { status = false, message = "You Are Trying Subscribe You Are Already Subscribed" });
                }


                var exiting_subscriber = _db_context.Subscribers.FirstOrDefault(x => x.UserId == user.Id &&
                x.Status == 0 &&
                x.SubscriptionEndDate <= DateTime.Now);

                if (exiting_subscriber != null)
                {

                    exiting_subscriber.Status = 1;
                    exiting_subscriber.SubscriptionStartDate = currentDate;
                    exiting_subscriber.SubscriptionEndDate = after30Days;

                    _db_context.SaveChanges();
                    return Json(new { status = true, message = "Subscription Activated Succesfully" });
                }
                else
                {
                    var subscriber = new Subscriber
                    {
                        UserId = user.Id,
                        Fee = "$500",
                        Status = 1,
                        SubscriptionStartDate = currentDate,
                        SubscriptionEndDate = after30Days,
                        Email = user.Email,
                        CreatedAt = DateTime.Now
                    };


                    var transaction = new Transaction
                    {
                        Name = user.Name,
                        Money = "$500",
                        At = DateTime.Now
                    };

                    _db_context.Transactions.Add(transaction);
                    _db_context.Subscribers.Add(subscriber);
                    _db_context.SaveChanges();
                    return Json(new { status = true, message = "Subscription Has Been Created Succesfully" });
                }


            }
            else
            {
                DateTime currentDate = DateTime.Now.Date;
                DateTime after30Days = currentDate.AddDays(30);
                var ipAddress = HttpContext.Connection.RemoteIpAddress;
                var formattedIpAddress = ipAddress.IsIPv4MappedToIPv6
                    ? ipAddress.MapToIPv4().ToString()
                    : ipAddress.ToString();

                var subscriber = _db_context.Subscribers.Where(x => x.IpAddress == formattedIpAddress  && x.SubscriptionEndDate > DateTime.Now).Select(e=> new
                {
                    e.Id,
                    e.Status,
                    e.IsGuest,
                    e.Email,
                    e.IpAddress
                }).FirstOrDefault();

                if (subscriber != null)
                {
                    return Json(new { status = false, message = "You Are Already Subscriber " });
                }
                var subscription = new Subscriber
                {
                    IsGuest = 1,
                    Email = email,
                    Fee = "$500",
                    SubscriptionStartDate = currentDate,
                    SubscriptionEndDate = after30Days,
                    IpAddress = formattedIpAddress,
                    Status = 1,
                    CreatedAt = DateTime.Now
                };

                //return Json(new { subscription });

                var transaction = new Transaction
                {
                    Name = "Guest",
                    Money = "$500",
                    At = DateTime.Now
                };

                //var exiting_subscriber = _db_context.Subscribers.Where(x => x.IpAddress == formattedIpAddress).Select(x=> new
                //{
                //    x.Id,
                //    x.IpAddress,
                //    x.Status,
                //    x.SubscriptionEndDate,
                //    x.SubscriptionStartDate
                //}).FirstOrDefault();
                //if(exiting_subscriber != null)
                //{
                //    var guestSubscriber = new Subscriber
                //    {
                //        Id = exiting_subscriber.Id,
                //        Status = 1,
                //        SubscriptionStartDate = currentDate,
                //        SubscriptionEndDate = after30Days,
                //    };

                //    _db_context.Attach(guestSubscriber);

                //    _db_context.Entry(guestSubscriber).Property(x => x.Status).IsModified = true;
                //    _db_context.Entry(guestSubscriber).Property(x => x.SubscriptionStartDate).IsModified = true;
                //    _db_context.Entry(guestSubscriber).Property(x => x.SubscriptionEndDate).IsModified = true;

                //    _db_context.Transactions.Add(transaction);

                //    _db_context.SaveChanges();

                //    return Json(new { status = true, message = "You Have Been Subscribed Successfully" });
                //}

                _db_context.Transactions.Add(transaction);
                _db_context.Subscribers.Add(subscription);
                _db_context.SaveChanges();


                return Json(new { status = true, message = "You Have Been Subscribed Succesfully" });

            }
        }


        [HttpPost]
        [Route("auth/check")]
        public IActionResult AuthCheck()
        {
            if (User.Identity.IsAuthenticated)
            {
                return Json(new { status = true });
            }
            else
            {
                return Json(new { status = false });
            }
        }
    }
}
