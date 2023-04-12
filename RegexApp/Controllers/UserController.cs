using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.AspNetCore.Mvc;
using RegexApp.Database;
using RegexApp.Models;
using System.Net;
using System.Xml.Linq;
using BC = BCrypt.Net.BCrypt;

namespace RegexApp.Controllers {
    public class UserController : Controller {

        private readonly Db db;
        private readonly IConfiguration _configuration;
        public UserController(Db db, IConfiguration configuration) {
            this.db = db;
            _configuration = configuration;
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
            if(userModel == null || userModel.Email == null || email != userModel.Email)
                return new ContentResult() { Content = "Invalid Email" };
                        
            //GENERAR EL TOKEN TEMPORAL
            TokenModel? tokenModel = TokenModel.GetTokenByUser(userModel.PK_Users, db);
            if (tokenModel == null) {
                TokenModel.AddToken(userModel.PK_Users, db);
                tokenModel = TokenModel.GetTokenByUser(userModel.PK_Users, db);
            }
            if (tokenModel == null) 
                return new ContentResult() { Content = "An error has occurred generating the token" };

            string protocol = Request.HttpContext.Request.Scheme;
            string host = Request.HttpContext.Request.Host.Value;
            string urlActual = $"{protocol}://{host}{"/User/ValidateCode"}?token={tokenModel.Token}";
            EmailModel e = new EmailModel();
            e.EmailTo = userModel.Email;
            e.Subject = "PASSWORD RESET";
            e.Body = $"To reset your password, please click this link {urlActual} and follow the steps"; /// PROTOCOLO - HOST - RUTA
            //MANDAR AL CORREO UNA URL CON EL TOKEN 
            if(EmailModel.SendEmail(e, _configuration)) 
                return RedirectToAction("Index", "Home");//Esta vista seria la que se le manda al correo valido con una validez de 30 minutos
            return View("Error");
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        [HttpGet]
        public ActionResult ValidateCode(Guid token) {
            //SET DE DATOS
            // Validar si el codigo temporal del parametro existe y el email
            
            TokenModel? t = TokenModel.GetToken(token, db);
            if (t != null) {
                return View("UserNewPassword", t);//Lo mando a ingresar su nueva contraseña 
            }
            return new ContentResult() { Content = "The code is not valid" };

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
        public ActionResult ChangePassword(Guid token, string password, string confirmPassword) {
            bool result = false;
            //VALIDACIONES DE CONTRASENA
            if (password != confirmPassword) {
                ModelState.AddModelError("ConfirmPassword", "Passwords do not match"); 
                return View("UserNewPassword");//Regresa a donde mismo CAMBIAR
            }
            //ENCRIPTAR LA CONTRASENA
            string encryptedPassword = BC.HashPassword(password);
            UserModel? user = UserModel.GetUserWithActiveToken(token, db);
            if (user == null) {
                ModelState.AddModelError("UserNotFound", "User not found");
                return View();//Regresa a donde mismo CAMBIAR
            }

            user.Password = encryptedPassword;
            result = UserModel.UpdateUserPassword(user, db);

            if(result)//SE HA ACTUALIZADO EL PASSWORD
                return RedirectToAction("Index", "Home");
            else//
                return RedirectToAction("Index", "Home");

            // QUIERO UNA FUNCION QUE ME DEVUELVA LOS NUMEROS PRIMOS MENORES A 100

        }


        // GET: UserController/Delete/5
        public ActionResult Delete(int id) {
            return View();
        }

    }
}
