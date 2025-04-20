using System.ComponentModel.DataAnnotations.Schema;

namespace E_Greetings.Models
{
    public class CardViewModel
    {
        public int Id { get; set; }
        public string? CardName { get; set; }
        public int Status { get; set; }

        public string? CardController { get; set; }

        public string? CardAction { get; set; }

    }
}
