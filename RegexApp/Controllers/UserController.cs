using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Xml.Linq;

namespace RegexApp.Controllers {
    public class UserController : Controller {
        // GET: UserController
        public ActionResult Index() {
            return View("User");
        }
        // GET: UserController/Details/5
        public ActionResult Details(int id) {
            return View();
        }

        // GET: UserController/ResetPassword
        public ActionResult ResetPassword() {
            return View("UserResetPassword");
        }

        [HttpPost]
        public ActionResult ValidateEmail(string email) {
            //SET DE DATOS
            string emailValido = "email.valido@gmail.com"; // Validar si el email del parametro existe
            if (email == emailValido) {
                //ENVIAR CODIGO AL emailValido
                return View("UserValidateTempCode");//Esta vista seria la que se le manda al correo valido con una validez de 30 minutos
            }
            return new ContentResult() { Content="Invalid Email"};
            
        }

        [HttpPost]
        public ActionResult ValidateCode(string code) {
            //SET DE DATOS
            string validCode = "000000"; // Validar si el codigo temporal del parametro existe
            if (code == validCode) {
                return View("UserNewPassword");//Lo mando a ingresar su nueva contraseña 
            }
            return new ContentResult() { Content = "The code is not valid" };

        }

        // GET: UserController/Create
        public ActionResult Create() {
            return View();
        }

        // POST: UserController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection) {
            try {
                return RedirectToAction(nameof(Index));
            }
            catch {
                return View();
            }
        }

        // GET: UserController/Edit/5
        public ActionResult Edit(int id) {
            return View();
        }

        // POST: UserController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection) {
            try {
                return RedirectToAction(nameof(Index));
            }
            catch {
                return View();
            }
        }

        // GET: UserController/Delete/5
        public ActionResult Delete(int id) {
            return View();
        }

        // POST: UserController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection) {
            try {
                return RedirectToAction(nameof(Index));
            }
            catch {
                return View();
            }
        }
    }
}
