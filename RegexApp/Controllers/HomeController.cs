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

namespace RegexApp.Controllers {
    public class HomeController : Controller {
          
        public IActionResult Index() {
            return View();
        }

        public IActionResult Privacy() {
            return View();
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