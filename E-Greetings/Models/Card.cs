using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E_Greetings.Models
{
    public class Card
    {


        [Key]
        public int Id { get; set; }

        [Column("card_list_id")]
        [Required(ErrorMessage = "Card Is Required")]
  
        public int Card_id { get; set; }




        [Column("status")]
        public int Status { get; set; }


        [ForeignKey("Card_id")]
        public virtual CardList? CardLists { get; set; } 

    }
}
