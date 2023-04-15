using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
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
            else {
                TempData["ErrorMessage"] = "Your password is incorrect or this user doesn't exist.";
                return RedirectToAction("Index", "Home");
            }
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
            if(userModel == null || userModel.Email == null || email != userModel.Email) {
                TempData["ErrorMessage"] = "Error validating the email";
                return new ContentResult() { Content = "Invalid Email" };
            }
                        
            //GENERAR EL TOKEN TEMPORAL
            TokenModel? tokenModel = TokenModel.GetTokenByUser(userModel.PK_Users, db);
            if (tokenModel == null) {
                TokenModel.AddToken(userModel.PK_Users, db);
                tokenModel = TokenModel.GetTokenByUser(userModel.PK_Users, db);
            }
            if (tokenModel == null ) 
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

        [HttpGet]
        public ActionResult ValidateCode(Guid token) {
            //SET DE DATOS
            // Validar si el codigo temporal del parametro existe y el email
            TokenModel? t = TokenModel.GetToken(token, db);
            if (t == null) {
                TempData["ErrorMessage"] = "This token is expired or is not available anymore";
                return View("UserNewPassword");//Lo mando a ingresar su nueva contraseña 
            }
            return View("UserNewPassword", t);//Lo mando a ingresar su nueva contraseña 
        }

        private string validaFormulario(IFormCollection form) {
            string errorMessage = "";
            foreach (var v in form) {
                if(v.Value.ToString() == "") {
                    errorMessage += $"Error, {v.Key} cannot be empty.\n";
                }
            }
            return errorMessage;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateUser(IFormCollection form) { //string username, string email, string password, string confirmPassword
            bool result = false;
            //VALIDACIONES DE CONTRASENA
            string message = validaFormulario(form);
            if (message != "") {
                TempData["ErrorMessage"] = message;
                return RedirectToAction("UserCreate");
            }
            if (form["password"] != form["confirmPassword"]) {
                TempData["ErrorMessage"] = "Error, the password and confirmation password does not match";
                return RedirectToAction("UserCreate");
            }
            //ENCRIPTAR LA CONTRASENA
            string encryptedPassword = BC.HashPassword(form["password"]);
            UserModel user = new UserModel() { FK_Users_Roles=2, Email = form["email"], UserName = form["username"], Password = encryptedPassword, Enabled = true};
            if(UserModel.ExistUser(user, db)) {
                TempData["ErrorMessage"] = "The username has been already taken, try with another username";
                return RedirectToAction("UserCreate");
            }

            result = UserModel.CreateUser(user, db);
            if (result) {
                TempData["SuccessMessage"] = $"The user {user.UserName} was created succesfully";
                return RedirectToAction("UserCreate");
            }
            else {
                TempData["ErrorMessage"] = "An error has occurred while creating the user";
                return RedirectToAction("UserCreate");
            }
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
            //VALIDACIONES DE CONTRASENA
            bool isUpdateComplete = false;
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
            isUpdateComplete = UserModel.UpdateUserPassword(user, db) && TokenModel.DisableToken(token, db);
            
            if(isUpdateComplete)//SE HA ACTUALIZADO EL PASSWORD MOSTRAR ALGUN TIPO DE ALERTA DE EXITO EN LA VISTA
                return RedirectToAction("Index", "Home");
            else//
                return RedirectToAction("Index", "Home");
        }


        // GET: UserController/Delete/5
        public ActionResult Delete(int id) {
            return View();
        }

    }
}
