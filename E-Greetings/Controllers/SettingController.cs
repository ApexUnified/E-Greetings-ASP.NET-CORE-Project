using System.ComponentModel;
using System.Security.Claims;
using E_Greetings.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using NuGet.Protocol;

namespace E_Greetings.Controllers
{

    [Authorize(Policy = "View Settings")]
    public class SettingController : Controller
    {

        private readonly EGreetingsContext _db_context;

        public SettingController(EGreetingsContext db_context)
        {
            _db_context = db_context;
        }

        //Role Settings Methods 

        [Authorize(Policy = "View Settings")]
        public IActionResult RoleSetting()
        {
            var roles = _db_context.Roles.ToList();
            return View(roles);
        }

        [Authorize(Policy = "View Settings")]
        public IActionResult RoleCreate()
        {
            return View();
        }

        [Authorize(Policy = "View Settings")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RoleCreate(Role role)
        {
            if (ModelState.IsValid)
            {
                _db_context.Roles.Add(role);
                _db_context.SaveChanges();
                TempData["Success"] = "Role Has Been Created Succesfully";
                return RedirectToAction("RoleSetting", "Setting");
            }


            return View(role);
        }

        [Authorize(Policy = "View Settings")]
        public IActionResult RoleEdit(int? id) 
        {
            if(id == null)
            {
                TempData["Error"] = "Id Is Required To Updated Role";
                return RedirectToAction("RoleSetting", "Setting");
            }

            var role = _db_context.Roles.FirstOrDefault(x => x.Id == id);
            if(role == null)
            {
                TempData["Error"] = "Role Not Found To Update";
                return RedirectToAction("RoleSetting", "Setting");
            }
            return View(role);
        }

        [Authorize(Policy = "View Settings")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RoleEdit(Role role, int? id)
        {
            if(id == null)
            {
                TempData["Error"] = "Role Id Missing For Updating";
            }

            if (ModelState.IsValid)
            {
                
                var existing_role = _db_context.Roles.FirstOrDefault(y => y.Id == id);
                //return Json(new
                //{
                //    existing_role
                //});
                if (existing_role == null) 
                {
                    TempData["Error"] = "Role Not Found To Update";
                    return RedirectToAction("RoleSetting", "Setting");
                }

                existing_role.Name = role.Name;
                _db_context.SaveChanges();
                TempData["Success"] = "Role Updated Succesfully";
                return RedirectToAction("RoleSetting", "Setting");

            }
            return View(role);
        }


        [Authorize(Policy = "View Settings")]
        public IActionResult RoleDestroy(int? id)
        {
            if(id == null)
            {
                TempData["Error"] = "Id Missing For Role To Delete";
                return RedirectToAction("RoleSetting", "Setting");
            }
            var role = _db_context.Roles.FirstOrDefault(x => x.Id == id);

            if(role == null)
            {
                TempData["Error"] = "Role Not Found To Delete";
                return RedirectToAction("RoleSetting", "Setting");
            }

            _db_context.Roles.Remove(role);
            _db_context.SaveChanges();
            TempData["Success"] = "Role Has Been Succesfully Deleted";
            return RedirectToAction("RoleSetting", "Setting");
        }


        [Authorize(Policy = "View Settings")]
        [Route("/role/deletebyselection")]
        [HttpPost]
        public IActionResult RolesDeleteBySelection(int[] role_ids)
        {

            if(role_ids.Contains(1) || role_ids.Contains(4))
            {
                return Json(new { status = false ,message = "You Cant Delete These Roles Its System Preserved" });
            }

            var roles = _db_context.Roles.Where(op => role_ids.Contains(op.Id)).ToList();
            if (roles == null || !roles.Any())
            {
                return Json(new { status = false, message = "No roles found for the provided IDs." });
            }
            try
            {
                _db_context.Roles.RemoveRange(roles);
                _db_context.SaveChanges();
                return Json(new { status = true, message = "Roles deleted successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { status = false, message = "An error occurred while deleting roles.", error = ex.Message });
            }
        }


        //Role Settings Methods 



        //RolePermission Settings Methods
        [Authorize(Policy = "View Settings")]
        public IActionResult RolePermissionsIndex(int? id) 
        { 
            if(id == null)
            {
                TempData["Error"] = "Id Is Missing Of Role";
                return View();
            }

            var role = _db_context.Roles.FirstOrDefault(y => y.Id == id);

            if(role == null)
            {
                TempData["Error"] = "Role Not Found ";
                return View();
            }

            var permissions = _db_context.Permissions.ToList();
            var assigned_permissions = _db_context.RolePermissions
                                      .Where(rp => rp.RoleId == id)
                                      .Select(rp => rp.PermissionId)
                                      .ToList();

            ViewData["assigned_permissions"] = assigned_permissions;
            ViewData["role_id"] = id;
            return View(permissions);
        }

        [Authorize(Policy = "View Settings")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RolePermissionsIndex(int[] Name, int role_id)
        {

            var existingPermissions = _db_context.RolePermissions
                                     .Where(rp => rp.RoleId == role_id)
                                     .ToList();

            if (existingPermissions.Any())  // Only remove if there are existing records
            {
                _db_context.RolePermissions.RemoveRange(existingPermissions);
                _db_context.SaveChanges();
            }

            var selectedIds = Name;

            foreach(var permissionId in Name)
            {
                var rolePermission = new RolePermission
                {
                    RoleId = role_id, 
                    PermissionId = permissionId 
                };

                _db_context.RolePermissions.Add(rolePermission);
                _db_context.SaveChanges();
            }

            TempData["Success"] = "Permissions Has Been Succesfully Assigned";
            return RedirectToAction("RoleSetting", "Setting");


  
        }

        [Authorize(Policy = "View Settings")]
        public IActionResult RolePermissionCreate()
        {
            return View();
        }

        [Authorize(Policy = "View Settings")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RolePermissionCreate(Permission permission)
        {
            if (ModelState.IsValid)
            {
               
                _db_context.Permissions.Add(permission);
                _db_context.SaveChanges();
                TempData["Success"] = "Permission Has Been Created For Roles";
                return View(permission);
            }

            return View(permission);
        }

        //RolePermission Settings Methods

    }
}
