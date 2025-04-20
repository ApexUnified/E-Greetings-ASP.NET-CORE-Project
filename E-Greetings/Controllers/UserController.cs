using E_Greetings.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_Greetings.Controllers
{

    [Authorize]
    public class UserController : Controller
    {

        private readonly EGreetingsContext _db_context;

        public UserController(EGreetingsContext db_context)
        {
            _db_context = db_context;
        }

        //Users Methods

        [Authorize(Policy = "View User")]
        public IActionResult UserIndex()
        {
            var users = _db_context.Users.Include(u => u.Role).ToList();
 
            return View(users);
        }

        [Authorize(Policy = "Create User")]
        public IActionResult UserCreate()
        {
            var roles = _db_context.Roles.ToList();
            ViewData["roles"] = roles;

            return View();
        }


        [Authorize(Policy = "Create User")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UserCreate(User user)
        {

            if (ModelState.IsValid)
            {
                if (user == null)
                {
                    TempData["Error"] = "Error Occured While Creating User";
                    return View(user);
                }

                var checkingEmail = _db_context.Users.Where(x => x.Email == user.Email).FirstOrDefault();
                var checkingUsername = _db_context.Users.Where(x => x.Name == user.Name).FirstOrDefault();
                if(checkingEmail != null)
                {
                    TempData["Error"] = "This Email is Already Taken";
                    var roless = _db_context.Roles.ToList();
                    ViewData["roles"] = roless;
                    return View(user);
                }

                if(checkingUsername != null)
                {
                    TempData["Error"] = "This UserName is Already Taken Please Choose Unique UserName";
                    var rolesss = _db_context.Roles.ToList();
                    ViewData["roles"] = rolesss;
                    return View(user);
                }



                var passwordHasher = new PasswordHasher<User>();
                user.Password = passwordHasher.HashPassword(user, user.Password);
                _db_context.Users.Add(user);
                _db_context.SaveChanges();
                TempData["Success"] = "User Has Been Created Succesfully";
                return RedirectToAction("UserIndex", "User");
            }

            var roles = _db_context.Roles.ToList();
            ViewData["roles"] = roles;

            return View(user);
        }


        [Authorize(Policy = "Edit User")]
        public IActionResult UserEdit(int? id)
        {

            if (id == null)
            {
                TempData["Error"] = "Id Cannot Be Null For User To Update";
                return RedirectToAction("UserIndex", "User");
            }

            var user = _db_context.Users.FirstOrDefault(x => x.Id == id);
            if (user == null)
            {
                TempData["Error"] = "user Cannot Be Found For The Given Id To Update";
                return RedirectToAction("UserIndex", "User");
            }

            var roles = _db_context.Roles.ToList();
            ViewData["roles"] = roles;

            var editUser = new EditUser
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                RoleId = user.RoleId
            };


            return View(editUser);
        }
        [Authorize(Policy = "Edit User")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UserEdit(EditUser E_user, int? id)
        {
            if (ModelState.IsValid)
            {
                var user = _db_context.Users.FirstOrDefault(u => u.Id == id);
                if (user == null)
                {
                    TempData["Error"] = "user Not Fount From Givin Id";
                    return RedirectToAction("UserIndex", "User");
                }

                if (user.Email != E_user.Email)
                {
                    var checkingEmail = _db_context.Users.Where(x => x.Email == E_user.Email).FirstOrDefault();
                    if (checkingEmail != null)
                    {
                        TempData["Error"] = "This Email is Already Taken";
                        var roless = _db_context.Roles.ToList();
                        ViewData["roles"] = roless;
                        return View(E_user);
                    }
                }

                    if(user.Name != E_user.Name)
                    {
                    var checkingUsername = _db_context.Users.Where(x => x.Name == E_user.Name).FirstOrDefault();
                    if (checkingUsername != null)
                    {
                        TempData["Error"] = "This UserName is Already Taken Please Choose Unique UserName";
                        var roless = _db_context.Roles.ToList();
                        ViewData["roles"] = roless;
                        return View(E_user);
                    }
                    }

                user.Name = E_user.Name;
                user.Email = E_user.Email;
                user.RoleId = E_user.RoleId;

                if(E_user.Password != null)
                {
                    var passwordHasher = new PasswordHasher<User>();
                    user.Password = passwordHasher.HashPassword(user, E_user.Password);
                }

                _db_context.SaveChanges();
                TempData["Success"] = "User Has Been Updated Succesfully";
                return RedirectToAction("UserIndex", "User");

            }
            var roles = _db_context.Roles.ToList();
            ViewData["roles"] = roles;


            return View(E_user);


        }


        [Authorize(Policy = "Delete User")]
        public IActionResult UserDestroy(int? id)
        {
            if (id == null)
            {
                TempData["Error"] = "id Is Not Found For User To Delete";
                return RedirectToAction("UserIndex", "User");
            }

            var user = _db_context.Users.FirstOrDefault(x => x.Id == id);
            if (user == null)
            {
                TempData["Error"] = "User Not Found For To Delete";
                return RedirectToAction("UserIndex", "User");

            }


            _db_context.Users.Remove(user);
            _db_context.SaveChanges();
            TempData["Success"] = "User Has Been Deleted Succesfully";
            return RedirectToAction("UserIndex", "User");

        }
        [Authorize(Policy = "Delete User")]
        [Route("users/deletebyselection")]
        [HttpPost]

        public IActionResult UserDeleteBySelection(int[] user_ids)
        {
            var users = _db_context.Users.Where(x => user_ids.Contains(x.Id)).ToList();

            if (users == null || !users.Any())
            {
                return Json(new { status = false, message = "user Not Found" });
            }

            try
            {

                _db_context.Users.RemoveRange(users);
                _db_context.SaveChanges();
                return Json(new { status = true, message = "users Has Been Deleted Succesfully" });

            }
            catch (Exception e)
            {
                return Json(new { status = false, message = e });
            }
        }




        //Users Methods End
    }
}
