using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace E_Greetings.Models;

public partial class Permission
{

    [Key]
    public int Id { get; set; }


    [Required(ErrorMessage = "Permission Name is Required")]
    public string? Name { get; set; }

    public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}
