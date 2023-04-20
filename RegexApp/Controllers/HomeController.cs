using Microsoft.AspNetCore.Mvc;
using RegexApp.Models;
using System.Diagnostics;
using System.Net.Mail;
using System.Net.Mime;
using System.Net;
using System.Configuration;
using Microsoft.Extensions.Configuration;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using RegexApp.Database;
using System.Security.Claims;

namespace RegexApp.Controllers {
    public class HomeController : Controller {

        private readonly Db _db;
        private readonly IConfiguration _configuration;

        public HomeController(Db db, IConfiguration configuration) {
            _db = db;
            _configuration = configuration;
        }

        public IActionResult Index() {
            return View();
        }

        public IActionResult Privacy() {
            return View();
        }

        // GET: UserController
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(UserModel user) {
            if (UserModel.ValidateUser(user, _db) && user.Username != null) {

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, "NombreUsuario"),
                    new Claim(ClaimTypes.Email, "usuario@dominio.com")
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                var authProperties = new AuthenticationProperties {
                    IsPersistent = true
                };
                HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
                
                return RedirectToAction("Index", "User");
            }
            else {
                TempData["ErrorMessage"] = "Your password is incorrect or this user doesn't exist.";
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult Email() {
            return View("Email");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}