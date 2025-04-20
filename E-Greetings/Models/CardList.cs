using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E_Greetings.Models
{

    [Table("cards_list")]
    public class CardList
    {

        [Key]
        public  int Id { get; set; }

        [Column("card_name")]
        public string? CardName { get; set; }



        [Column("controller")]
        public string? CardController { get; set; }

        [Column("action")]
        public string? CardAction { get; set; }

        public virtual ICollection<Card>? Cards { get; set; }

        public virtual ICollection<CardSent>? CardSents { get; set; }
    }
}
