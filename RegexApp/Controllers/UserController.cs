using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RegexApp.Database;
using RegexApp.Models;
using BC = BCrypt.Net.BCrypt;

namespace RegexApp.Controllers {
    [Authorize]
    public class UserController : Controller {

        private readonly Db _db;
        private readonly IConfiguration _configuration;
        public UserController(Db db, IConfiguration configuration) {
            _db = db;
            _configuration = configuration;
        }

        // GET: /User
        public ActionResult Index() {
            if (User != null && User.Identity != null && User.Identity.IsAuthenticated) {
                return View();
            }// El usuario no está autenticado
            else {
                return RedirectToAction("Index", "Home");
            }
        }


        [HttpGet]
        public ActionResult ValidateCode(Guid token) {
            //SET DE DATOS
            // Validar si el codigo temporal del parametro existe y el email
            TokenModel? t = TokenModel.GetToken(token, _db);
            if (t == null) {
                TempData["ErrorMessage"] = "This token is expired or is not available anymore";
                return View("UserNewPassword");//Lo mando a ingresar su nueva contraseña 
            }
            return View("UserNewPassword", t);//Lo mando a ingresar su nueva contraseña 
        }
               
        [HttpGet] // GET: User/GetPartialView
        public IActionResult GetPartialView(string view) {
            PartialViewResult partialView = PartialView($"Partials/{view}");
            if (partialView == null)
                return PartialView("Partials/_EmptyViewResult");
            return partialView; 
        }



    }
}
