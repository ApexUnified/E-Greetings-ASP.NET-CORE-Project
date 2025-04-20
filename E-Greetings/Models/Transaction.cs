using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E_Greetings.Models
{

    [Table("transactions")]
    public class Transaction
    {
        [Key]
        public int Id { get; set; }

        public string? Name { get; set; }

        public string? Money { get; set; }

        public DateTime At {  get; set; }
    }
}
