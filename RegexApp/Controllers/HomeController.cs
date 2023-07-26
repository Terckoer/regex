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
using BC = BCrypt.Net.BCrypt;
using RegexApp.Filters;

namespace RegexApp.Controllers {
    public class HomeController : Controller {

        private readonly Db _db;
        private readonly IConfiguration _configuration;

        public HomeController(Db db, IConfiguration configuration) {
            _db = db;
            _configuration = configuration;
        }

        [HttpGet] // GET: /
        public IActionResult Index() {
            return View();
        }

        [HttpGet] // GET: /Home/Privacy
        public IActionResult Privacy() {
            return View();
        }

        [HttpGet] // GET: /Login
        [RedirectAuthenticatedUserFilter]
        [Route("Login")]
        public IActionResult Login() {
            return View();
        }

        [HttpGet] // GET: /Home/UserCreate
        public ActionResult UserCreate() {
            return View("UserCreate");
        }

        // POST: /Home/ValidateUser
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ValidateUser(UserModel user) {
            if (UserModel.ValidateUser(user, _db) && user.Username != null) {
                List<Claim> claims = new List<Claim> { 
                    new Claim(ClaimTypes.Name, user.Username) 
                };

                ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                AuthenticationProperties authProperties = new AuthenticationProperties {
                    IsPersistent = true,
                    ExpiresUtc = DateTime.UtcNow.AddDays(7),
                };
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
                HttpContext.Session.SetString("username", user.Username);
                
                if (Request.Cookies["username"] == null) {
                    var cookieOptions = new CookieOptions {
                        Expires = DateTime.UtcNow.AddDays(7),
                        IsEssential = true ,
                        Secure = true,
                        SameSite = SameSiteMode.Lax
                    };
                    Response.Cookies.Append("username", user.Username, cookieOptions);
                }

                return RedirectToAction("Index", "User");
            }
            else {
                TempData["ErrorMessage"] = "Your password is incorrect or this user doesn't exist.";
                return RedirectToAction("Login", "Home");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Logout(UserModel user) {
            await HttpContext.SignOutAsync();
            Response.Cookies.Delete("username");
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");    
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateUser(IFormCollection form) { //string username, string email, string password, string confirmPassword
            bool result = false;
            //VALIDACIONES DE CONTRASENA
            string message = validateForm(form);
            if (message != "") {
                TempData["ErrorMessage"] = message;
                return RedirectToAction("UserCreate", "Home");
            }
            if (form["password"] != form["confirmPassword"]) {
                TempData["ErrorMessage"] = "Error, the password and confirmation password does not match";
                return RedirectToAction("UserCreate", "Home");
            }
            //ENCRIPTAR LA CONTRASENA
            string encryptedPassword = BC.HashPassword(form["password"]);
            UserModel user = new UserModel() { FK_Users_Roles = 2, Email = form["email"], Username = form["username"], Password = encryptedPassword, Enabled = true };
            if (UserModel.ExistUser(user, _db)) {
                TempData["ErrorMessage"] = "The username has been already taken, try with another username";
                return RedirectToAction("UserCreate", "Home");
            }

            result = UserModel.CreateUser(user, _db);
            if (result) {
                TempData["SuccessMessage"] = $"The user {user.Username} was created succesfully";
                return RedirectToAction("Login", "Home");
            }
            else {
                TempData["ErrorMessage"] = "An error has occurred while creating the user";
                return RedirectToAction("UserCreate", "Home");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ValidateEmail(string email) {
            //SET DE DATOS
            UserModel? userModel = UserModel.GetUser(email, _db); // Validar si el email del parametro existe
            if (userModel == null || userModel.Email == null || email != userModel.Email) {
                TempData["ErrorMessage"] = "Error validating the email";
                return new ContentResult() { Content = "Invalid Email" };
            }

            //GENERAR EL TOKEN TEMPORAL
            TokenModel? tokenModel = TokenModel.GetTokenByUser(userModel.PK_Users, _db);
            if (tokenModel == null) {
                TokenModel.AddToken(userModel.PK_Users, _db);
                tokenModel = TokenModel.GetTokenByUser(userModel.PK_Users, _db);
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
            if (EmailModel.SendEmail(e, _configuration))
                return RedirectToAction("Index", "Home");//Esta vista seria la que se le manda al correo valido con una validez de 30 minutos
            return View("Error");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
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
            UserModel? user = UserModel.GetUserWithActiveToken(token, _db);
            if (user == null) {
                ModelState.AddModelError("UserNotFound", "User not found");
                return View();//Regresa a donde mismo CAMBIAR
            }

            user.Password = encryptedPassword;
            isUpdateComplete = UserModel.UpdateUserPassword(user, _db) && TokenModel.DisableToken(token, _db);

            if (isUpdateComplete)//SE HA ACTUALIZADO EL PASSWORD MOSTRAR ALGUN TIPO DE ALERTA DE EXITO EN LA VISTA
                return RedirectToAction("Index", "Home");
            else//
                return RedirectToAction("Index", "Home");
        }


        public ActionResult ResetPassword() {
            return View("UserResetPassword");
        }

        /* NO ACTIONS*/
        private string validateForm(IFormCollection form) {
            string errorMessage = "";
            foreach (var v in form) {
                if (v.Value.ToString() == "") {
                    errorMessage += $"Error, {v.Key} cannot be empty.\n";
                }
            }
            return errorMessage;
        }
    }
}