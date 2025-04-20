using System.ComponentModel.DataAnnotations.Schema;

namespace E_Greetings.Models
{
    public class CardSentViewModel
    {

        public int Id { get; set; }

        public string? SentFrom { get; set; }

        public string? SentTo { get; set; }

        public DateTime? SentAt { get; set; }

        public int Card_Id { get; set; }
        public string  User { get; set; }

        public string  Card { get; set; }


        public string? Created_at { get; set; }
    }
}
