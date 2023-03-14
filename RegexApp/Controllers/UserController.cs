using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RegexApp.Models;
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
            UserModel? userModel = UserModel.GetUser(email); // Validar si el email del parametro existe
            string validEmail = "";
            string validCode = "";
            if(userModel != null) {
                validEmail = userModel.Email ?? "";
                validCode = userModel.GetRandomNumber();
            }
            ViewData["ValidCode"] = validCode;
            
            if (email == validEmail)//ENVIAR CODIGO AL emailValido
                return View("UserValidateTempCode");//Esta vista seria la que se le manda al correo valido con una validez de 30 minutos
            
            return new ContentResult() { Content="Invalid Email"};
            
        }

        [HttpPost]
        public ActionResult ValidateCode(string code, string validCode) {
            //SET DE DATOS
            // Validar si el codigo temporal del parametro existe
            if (code == validCode) {
                return View("UserNewPassword");//Lo mando a ingresar su nueva contraseña 
            }
            return new ContentResult() { Content = "The code is not valid" };

        }

        // GET: UserController/Create
        public ActionResult CreateUser(string username, string email, string password, string confirmPassword) {
            //VALIDACIONES DE CONTRASENA
            //ENCRIPTAR LA INFORMACION
            //CreateUser
            UserModel user = new UserModel() { Email = email, UserName = username, Password = password};



            return View();
        }

        public ActionResult GetUserCreate() {
            return View("UserCreate");
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
