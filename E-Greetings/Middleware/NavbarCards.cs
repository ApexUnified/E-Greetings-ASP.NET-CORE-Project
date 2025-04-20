using E_Greetings.Models;
using Microsoft.EntityFrameworkCore;
using static System.Net.Mime.MediaTypeNames;

namespace E_Greetings.Middleware
{
    public class NavbarCards
    {
        private readonly RequestDelegate _next;

        public NavbarCards(RequestDelegate next)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Use the scoped service provider for the DbContext
            var dbContext = context.RequestServices.GetService<E_Greetings.Models.EGreetingsContext>();

            if (dbContext != null)
            {
                var cards = dbContext.Cards
                    .Include(u => u.CardLists)
                    .Where(x => x.Status == 1)
                    .Select(x => new CardViewModel
                    {
                        Id = x.CardLists != null ? x.CardLists.Id : 0,
                        CardName = x.CardLists != null ? x.CardLists.CardName : null,
                        CardAction = x.CardLists != null ? x.CardLists.CardAction : null,
                        CardController = x.CardLists != null ? x.CardLists.CardController : null,
                    }).ToList();

                // Store the cards in HttpContext.Items
                context.Items["NavbarCards"] = cards;
            }

            await _next(context);
        }
    }

}
