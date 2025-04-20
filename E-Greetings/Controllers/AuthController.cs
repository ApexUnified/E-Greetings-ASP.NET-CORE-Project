using E_Greetings.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System.Security.Claims;
using System.Security.Principal;

namespace E_Greetings.Controllers
{
    public class AuthController : Controller
    {

        private readonly EGreetingsContext _db_context;

        public AuthController(EGreetingsContext db_context)
        {
            _db_context = db_context;
        }
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("DashboardIndex", "Dashboard");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string Email, string Password)
        {
            if (ModelState.IsValid)
            {

                var user = _db_context.Users.Where(u => u.Email == Email)
                .Select( u => new
                {
                    u.Id,
                    u.Email,
                    u.Name,
                    u.Password,
                    Role = u.Role.Name
              
                }).FirstOrDefault();


                if (user == null)
                {
                    TempData["Error"] = "Invalid Credentials";
                    return RedirectToAction("Login", "Auth");
                }

                var passwordHasher = new PasswordHasher<User>();
                var verificationResult = passwordHasher.VerifyHashedPassword(new User(), user.Password, Password);
                  
                if (verificationResult == PasswordVerificationResult.Success)
                {

                  


                    var claims = new List<Claim>
                    {
                         new Claim(ClaimTypes.Name, user.Name),
                         new Claim(ClaimTypes.Email, user.Email),
                         new Claim(ClaimTypes.Role, user.Role),
                    };

                    var Identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var Principle = new ClaimsPrincipal(Identity);
                   

                    await HttpContext.SignInAsync("MyAuth", Principle);

                   if(user.Role == "User")
                    {
                        return RedirectToAction("Home", "Website");
                    }

                    //HttpContext.Session.SetString("Role", user.Role);
                    //HttpContext.Session.SetString("Email", user.Email);
                    //HttpContext.Session.SetString("Id", Convert.ToString(user.Id));
                    TempData["Auth"] = "Welcome Back " + User.Identity.Name;
                    return RedirectToAction("DashboardIndex", "Dashboard");

                }
                else
                {
                    TempData["Error"] = "Invalid Credentials";
                    return RedirectToAction("Login", "Auth");
                }
            }

            if (Email == null)
            {
                ViewBag.Error_Email = "Email Field is Required";
            }
            if(Password == null)
            {
                ViewBag.Error_Password = "Password Field is Required";
            }




            return View();
        }

        public IActionResult Register()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("DashboardIndex", "Dashboard");
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(User user)
        {

            if (ModelState.IsValid) 
            {
                if (user != null)
                {
                    var exiting_Email = _db_context.Users.Where(e => e.Email == user.Email).FirstOrDefault();
                    var exiting_Username = _db_context.Users.Where(e => e.Name == user.Name).FirstOrDefault();
                    if (exiting_Email != null)
                    {
                        TempData["Error"] = "This Email is Already Registered Please Choose Another Email";
                        return View(user);
                    }

                    if(exiting_Username != null)
                    {
                        TempData["Error"] = "This Username is Already Registered Please Choose Another Username";
                        return View(user);
                    }

                    var passwordHasher = new PasswordHasher<User>();
                    user.Password = passwordHasher.HashPassword(user, user.Password);
                    user.RoleId = 4;

                    _db_context.Users.Add(user);
                    _db_context.SaveChanges();
                    TempData["Registered"] = "Thanks For Registering Now You Can Login";
                    return RedirectToAction("Login", "Auth");
                }
            }
            return View(user);
        }


        public IActionResult Logout()
        {
            HttpContext.SignOutAsync("MyAuth");
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Auth");
        }


        public IActionResult AccessDenied()
        {
            return View();
        }

    }
}
