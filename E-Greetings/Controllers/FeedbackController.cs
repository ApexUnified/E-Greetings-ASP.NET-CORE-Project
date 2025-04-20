using E_Greetings.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_Greetings.Controllers
{
    [Authorize(Policy = "View Feedbacks")]
    public class FeedbackController : Controller
    {

        private readonly EGreetingsContext _db_context;

        public FeedbackController(EGreetingsContext db_context)
        {
            _db_context = db_context;
        }


        [Authorize(Policy = "View Feedbacks")]
        public IActionResult Index()
        {
            var feedbacks = _db_context.Feedbacks.ToList();
            return View(feedbacks);
        }


        [Authorize(Policy = "Edit Feedbacks")]
        public IActionResult Edit(int? id)
        {
            if (id == null) {

                TempData["Error"] = "Id Is Missing Of Feedback";
                return RedirectToAction("Index","Feedback");
            }

            var feedback = _db_context.Feedbacks.FirstOrDefault(x => x.Id == id);

            if(feedback == null)
            {
                TempData["Error"] = "Feedback Is Missing ";
                return RedirectToAction("Index", "Feedback");
            }

            return View(feedback);
        }


        [Authorize(Policy = "Edit Feedbacks")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Feedback feedback)
        {
            if (id == null)
            {

                TempData["Error"] = "Id Is Missing Of Feedback";
                return RedirectToAction("Index", "Feedback");
            }

            if (ModelState.IsValid)
            {
                var feedbackk = _db_context.Feedbacks.FirstOrDefault(x => x.Id == id);

                if (feedbackk == null)
                {
                    TempData["Error"] = "Feedback Is Missing ";
                    return RedirectToAction("Index", "Feedback");
                }

                feedbackk.Name = feedback.Name;
                feedbackk.Email = feedback.Email;
                feedbackk.Message = feedback.Message;
                feedbackk.Status = feedback.Status;

                _db_context.SaveChanges();

                TempData["Success"] = "Feedback Has Been Updated";
                return RedirectToAction("Index", "Feedback");

            }
            TempData["Error"] = "Error Occured While Updating Feedback";
            return View(feedback);
        }


        [Authorize(Policy = "Delete Feedbacks")]
        public IActionResult Destroy(int? id)
        {
            if (id == null)
            {

                TempData["Error"] = "Id Is Missing Of Feedback";
                return RedirectToAction("Index", "Feedback");
            }

            var feedbackk = _db_context.Feedbacks.FirstOrDefault(x => x.Id == id);

            if (feedbackk == null)
            {
                TempData["Error"] = "Feedback Is Missing ";
                return RedirectToAction("Index", "Feedback");
            }

            _db_context.Feedbacks.Remove(feedbackk);
            _db_context.SaveChanges();
            TempData["Success"] = "Feedback Has Been Deleted";
            return RedirectToAction("Index", "Feedback");
        }

        [Authorize(Policy = "Delete Feedbacks")]
        [Route("feedbacks/deletebyselection")]
        [HttpPost]
        public IActionResult DestroyBySelection(int[]? ids) 
        {

            if (ids == null)
            {
                return Json(new { status = false });
            }

            var feedbacks = _db_context.Feedbacks.Where(x=> ids.Contains(x.Id)).ToList();
            //return Json(new { feedbacks });
            if(feedbacks == null)
            {
                return Json(new { status = false });
            }

           foreach(var feedback in feedbacks)
            {
                _db_context.Feedbacks.Remove(feedback);
            }
            _db_context.SaveChanges();

            return Json(new { status = true });
        }
    }
}
