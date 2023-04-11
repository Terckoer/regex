﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RegexApp.Models;
using System.Configuration;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;

namespace RegexApp.Controllers {
    public class EmailController : Controller {
        // GET: EmailController
        private readonly IConfiguration _configuration;

        public EmailController(IConfiguration configuration) {
            _configuration = configuration;
            var result = _configuration.GetValue<string>("ConnectionStrings:conn");
        }

        public ActionResult Index() {
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
                
                return RedirectToAction(nameof(Index));
            }
            catch {
                return View();
            }
        }


        

    }
}
