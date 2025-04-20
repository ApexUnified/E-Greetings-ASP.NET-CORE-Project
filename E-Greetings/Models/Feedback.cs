using System.ComponentModel.DataAnnotations;

namespace E_Greetings.Models
{

    public class Feedback
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string? Name { get; set; }


        [Required]
        [EmailAddress]
        public string? Email { get; set; }


        [Required]
        public string? Message { get; set; }


        public int Status { get; set; }
    }
}
