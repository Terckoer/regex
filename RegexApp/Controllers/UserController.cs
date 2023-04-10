using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RegexApp.Database;
using RegexApp.Models;
using System.Net;
using System.Xml.Linq;
using BC = BCrypt.Net.BCrypt;

namespace RegexApp.Controllers {
    public class UserController : Controller {

        private readonly Db db;
        public UserController(Db db) {
            this.db = db;
        }

        // GET: UserController
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(UserModel user) {
            if (UserModel.ValidateUser(user, db)) {
                ViewData["username"] = user.UserName;
                return View();
            }
            else
                return RedirectToAction("Index", "Home");
        }

        public ActionResult Details(int id) {
            return View();
        }

        public ActionResult ResetPassword() {
            return View("UserResetPassword");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ValidateEmail(string email) {
            //SET DE DATOS
            UserModel? userModel = UserModel.GetUser(email, db); // Validar si el email del parametro existe
            if (userModel != null && userModel.Email != null && email == userModel.Email) {
                /*
                 Si la dirección de correo electrónico existe, debes generar un token único y aleatorio que se utilizará para identificar al usuario 
                 y permitirle restablecer su contraseña. Este token debe tener una fecha de caducidad, por lo que debes almacenarlo junto con la fecha
                 y hora de su creación en la base de datos. Puedes utilizar la función "NEWID()" en SQL Server para generar un token aleatorio.
                 */
                //GENERAR EL TOKEN TEMPORAL
                TokenModel? tokenModel = TokenModel.GetTokenByUser(userModel.PK_Users, db);
                if (tokenModel == null) 
                    TokenModel.AddToken(userModel.PK_Users, db);

                //MANDAR AL CORREO UNA URL CON EL TOKEN 
                return RedirectToAction("Index", "Home");//Esta vista seria la que se le manda al correo valido con una validez de 30 minutos
            }
            return new ContentResult() { Content="Invalid Email"};
            
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        [HttpGet]
        public ActionResult ValidateCode(string token="") {
            //SET DE DATOS
            // Validar si el codigo temporal del parametro existe y el email
            TokenModel? t = TokenModel.GetToken(token, db);
            if (t != null) {
                return View("UserNewPassword");//Lo mando a ingresar su nueva contraseña 
            }
            return new ContentResult() { Content = "The code is not valid" };

        }

        [HttpGet]
        public ActionResult OtraUrl() {
            return RedirectToAction("Index","Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateUser(string username, string email, string password, string confirmPassword) {
            bool result = false;
            //VALIDACIONES DE CONTRASENA
            if(password == confirmPassword) {
                //ENCRIPTAR LA CONTRASENA
                string encryptedPassword = BC.HashPassword(password);
                UserModel user = new UserModel() { FK_Users_Roles=2, Email = email, UserName = username, Password = encryptedPassword, Enabled = true};
                result = UserModel.CreateUser(user, db);
            }
            if (result)
                return RedirectToAction("Index", "Home");
            else
                return RedirectToAction("UserCreate");
        }

        public ActionResult UserCreate() {
            return View("UserCreate");
        }

        public ActionResult Logout() {
            //CERRAR SESION, ELIMINAR COOKIES, TOKENS Y DEMAS COSAS QUE SIRVAN PARA VALIDAR SESION
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(string email, string password, string confirmPassword) {
            bool result = false;
            string username = "";//BUSCAR EL USUARIO QUE TIENE EL CODIGO "N" Y QUE TIENE EL CORREO email@example.com
            //VALIDACIONES DE CONTRASENA
            if (password == confirmPassword) {
                //ENCRIPTAR LA CONTRASENA
                string encryptedPassword = BC.HashPassword(password);
                UserModel user = new UserModel() { Email = email, UserName = username, Password = encryptedPassword};
                result = UserModel.UpdateUserPassword(user, db);
            }
            if (result)
                return RedirectToAction("Index", "Home");
            else
                return RedirectToAction("UserCreate");
        }

        // POST: UserController/Edit/5
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit(int id, IFormCollection collection) {
        //    try {
        //        return RedirectToAction(nameof(Index));
        //    }
        //    catch {
        //        return View();
        //    }
        //}

        // GET: UserController/Delete/5
        public ActionResult Delete(int id) {
            return View();
        }

        // POST: UserController/Delete/5
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Delete(int id, IFormCollection collection) {
        //    try {
        //        return RedirectToAction(nameof(Index));
        //    }
        //    catch {
        //        return View();
        //    }
        //}
    }
}
