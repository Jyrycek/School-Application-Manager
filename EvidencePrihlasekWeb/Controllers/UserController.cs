using EvidencePrihlasekWeb.Models;
using EvidencePrihlasekWeb.Services;
using Microsoft.AspNetCore.Mvc;
using Database;

namespace EvidencePrihlasekWeb.Controllers
{
    public class UserController : Controller
    {
        private AuthService _authService;

        public UserController(AuthService authService)
        {
            DatabaseManager.connectionString = "Data Source=../test.db";
            _authService = authService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (HttpContext.User.Identity == null)
            {
                return View();
            }

            if (HttpContext.User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Dashboard", "Dashboard");
            }

            return View();
        }
        [HttpGet]
        public IActionResult Registration()
        {
            if (HttpContext.User.Identity == null)
            {
                return View();
            }

            if (HttpContext.User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Dashboard", "Dashboard");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (!ModelState.IsValid) return View();

            Users user = await DatabaseManager.GetUserByUsername<Users>(model.Username);

            if (user == null)
            {
                ModelState.AddModelError("Username", "Uživatelské jméno neexistuje");
                return View();
            }

            bool isValidPassword = BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash);

            if (!isValidPassword)
            {
                ModelState.AddModelError("Password", "Heslo je nesprávné");
                return View();
            }

            await _authService.LoginUser(user, HttpContext);
            return RedirectToAction("Dashboard", "Dashboard");
        }

        [HttpPost]
        public async Task<IActionResult> Register(Users model)
        {
            if (!ModelState.IsValid) return View("Registration");

            int email_number = await DatabaseManager.CountUsersAsync<Users>(nameof(model.Email), model.Email);
            int username_number = await DatabaseManager.CountUsersAsync<Users>(nameof(model.Username), model.Username);


            if (email_number > 0)
            {
                ModelState.AddModelError("Email", "Email už existuje");
            }
            else if (model.PasswordHash != model.Password2)
            {
                ModelState.AddModelError("Password2", "Zadané hesla nesouhlasí");
            }
            else if (username_number > 0)
            {
                ModelState.AddModelError("Username", "Zadané uživatelské jméno existuje");
            }
            else
            {
                await _authService.RegisterUser(new Users()
                {
                    Username = model.Username,
                    Email = model.Email,
                    PasswordHash = model.PasswordHash
                });

                return RedirectToAction("Login", "User");
            }
            return View("Registration");
        }
    }
}
