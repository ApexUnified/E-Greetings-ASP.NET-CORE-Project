using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E_Greetings.Models
{


    [Table("cards_sent")]
    public class CardSent
    {

        [Key]
        public int Id { get; set; }


        [Column("user_name")]
        public string? User_Name { get; set; }


        [Column("sent_from")]
        public string? SentFrom { get; set; }

        [Column("sent_to")]
        public string? SentTo { get; set; }


        [Column("sent_at")]
        public DateTime? SentAt { get; set; }




        [Column("card_id")]
        public int Card_Id { get; set; }


        [Column("created_at")]
        public DateTime CreatedAt { get; set; }


        [ForeignKey("Card_Id")]
        public virtual CardList Card { get; set; }
    }
}
