using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E_Greetings.Models;

public partial class User
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
    [Required(ErrorMessage = "Password Field is Required")]
    [Compare("ConfirmPassword",ErrorMessage = "Password Do Not Match With Confirm Password")]
    public string? Password { get; set; }

    [DataType(DataType.Password)]
    [NotMapped]
    [Required(ErrorMessage = "Confirm Password Field is Required")]
    public string? ConfirmPassword { get; set; }
    public int? RoleId { get; set; }

    public virtual Role? Role { get; set; }
    public virtual Subscriber? Subscriber { get; set; }

}
