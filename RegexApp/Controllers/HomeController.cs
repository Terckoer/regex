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
using NuGet.Protocol;

namespace RegexApp.Controllers {
    public class HomeController : Controller {

        private readonly Db _db;
        private readonly IConfiguration _configuration;

        public HomeController(Db db, IConfiguration configuration) {
            _db = db;
            _configuration = configuration;
        }

        public IActionResult Index() {
            CargarViewBags();
            return View();
        }

        public IActionResult Privacy() {
            CargarViewBags();
            return View();
        }

        // GET: UserController
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(UserModel user) {
            if (UserModel.ValidateUser(user, _db) && user.Username != null) {
                List<Claim> claims = new List<Claim> { 
                    new Claim(ClaimTypes.Name, user.Username) 
                };

                ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                AuthenticationProperties authProperties = new AuthenticationProperties {
                    IsPersistent = true,
                };
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
                HttpContext.Session.SetString("username", user.Username);
                
                if (Request.Cookies["username"] == null) {
                    var cookieOptions = new CookieOptions {
                        Expires = DateTime.UtcNow.AddDays(7),
                        IsEssential = true 
                    };
                    Response.Cookies.Append("username", user.Username, cookieOptions);
                }

                return RedirectToAction("Index", "User");
            }
            else {
                TempData["ErrorMessage"] = "Your password is incorrect or this user doesn't exist.";
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Logout(UserModel user) {
            await HttpContext.SignOutAsync();
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");    
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public void CargarViewBags() {
            string? miCookie = Request.Cookies["authenticatedUser"];
            if (miCookie != null) { 
                ViewBag.Username = Request.Cookies["username"];
            }
        }

    }
}