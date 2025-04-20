
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E_Greetings.Models
{
    public class EditUser
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Name Field Is Required")]
        [MinLength(2)]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Email Field Is Required")]
        [EmailAddress(ErrorMessage = "This Field Data In Must Be Email Address Format")]
        public string? Email { get; set; }


        [DataType(DataType.Password)]
        [Compare("ConfirmPassword", ErrorMessage = "Password Do Not Match With Confirm Password")]
        public string? Password { get; set; }

        [DataType(DataType.Password)]
        [NotMapped]
        public string? ConfirmPassword { get; set; }


        [Required(ErrorMessage = "Role is Required")]
        public int? RoleId { get; set; }
    }
}
