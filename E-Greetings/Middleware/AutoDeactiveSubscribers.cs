using E_Greetings.Models;
using Microsoft.EntityFrameworkCore;

namespace E_Greetings.Middleware
{
    public class AutoDeactiveSubscribers
    {
        private readonly RequestDelegate _next;

        public AutoDeactiveSubscribers(RequestDelegate next)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Use the scoped service provider for the DbContext
            var dbContext = context.RequestServices.GetService<E_Greetings.Models.EGreetingsContext>();

            if (dbContext != null)
            {

                var subscribers = dbContext.Subscribers.Where(x => x.Status == 1 && x.SubscriptionEndDate <= DateTime.Now && x.IsGuest == 0).ToList();
                if (subscribers.Any())
                {
                    foreach (var subscriber in subscribers)
                    {

                        subscriber.Status = 0;
                    }
                    await dbContext.SaveChangesAsync();
                }


                var guests = dbContext.Subscribers
                    .Where(x => x.Status == 1 && x.SubscriptionEndDate <= DateTime.Now && x.IsGuest == 1)
                    .Select(x => new
                    {
                        x.Id,
                        x.Status,
                        x.SubscriptionEndDate,
                        x.Email 
                    })
                    .ToList();

                if (guests.Any())
                {
                    foreach (var guest in guests)
                    {
                        var guestSubscriber = new Subscriber { Id = guest.Id, Status = 0 };
                        dbContext.Attach(guestSubscriber);
                        dbContext.Entry(guestSubscriber).Property(x => x.Status).IsModified = true;
                    }

                    await dbContext.SaveChangesAsync();
                }


            }

            await _next(context);
        }
    }
}
