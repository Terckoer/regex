using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RegexApp.Models;
using System.Configuration;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;

namespace RegexApp.Controllers {
    [Authorize]
    public class EmailController : Controller {
        // GET: EmailController
        private readonly IConfiguration _configuration;

        public EmailController(IConfiguration configuration) {
            _configuration = configuration;
            var result = _configuration.GetValue<string>("ConnectionStrings:conn");
        }

        public ActionResult Index() {
            CargarViewBags();
            return View();
        }

        public ActionResult Details(int id) {
            return View();
        }

        public ActionResult Create() {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection) {
            try {
                EmailModel e = new EmailModel();
                e.EmailTo = collection["to"];
                e.Subject = collection["subject"];
                e.Body = collection["body"];
                
                EmailModel.SendEmail(e, _configuration);
                
                
                return RedirectToAction(nameof(Index));
            }
            catch {
                return RedirectToAction("Index","Home");
            }
        }
        
        public void CargarViewBags() {
            string? miCookie = Request.Cookies["authenticatedUser"];
            if (miCookie != null) {
                ViewBag.Username = Request.Cookies["username"];
            }
        }
    }
}
