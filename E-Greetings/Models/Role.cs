using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace E_Greetings.Models;

public partial class Role
{

    [Key]
    public int Id { get; set; }


    [Required(ErrorMessage = "Role Name Is Required")]
    public string? Name { get; set; }

    public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
