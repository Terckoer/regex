using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;

namespace RegexApp.Controllers {
    public class EmailController : Controller {
        // GET: EmailController
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
                return RedirectToAction(nameof(Index));
            }
            catch {
                return View();
            }
        }


        private void sendEmail() {
            MailMessage email = new MailMessage();
            SmtpClient smtp = new SmtpClient();

            email.To.Add(new MailAddress("correo@destino.com"));
            email.From = new MailAddress("correo@origen.com");
            email.Subject = "Notificación ( " + DateTime.Now.ToString("dd / MMM / yyy hh:mm:ss") + " ) ";
            email.SubjectEncoding = System.Text.Encoding.UTF8;
            email.Body = "Tu mensaje | tu firma";
            email.IsBodyHtml = true;
            email.Priority = MailPriority.Normal;
            FileStream fs = new FileStream("E:\\TestFolder\\test.pdf", FileMode.Open, FileAccess.Read);
            Attachment a = new Attachment(fs, "test.pdf", MediaTypeNames.Application.Octet);
            email.Attachments.Add(a);

            smtp.Host = "192.XXX.X.XXX";  // IP empresa/institucional
                                          //smtp.Host = "smtp.hotmail.com";
                                          //smtp.Host = "smtp.gmail.com";
            smtp.Port = 25;
            smtp.Timeout = 50;
            smtp.EnableSsl = false;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new NetworkCredential("correo@origen.com", "password");

            string lista = "ejemplo1@correo.com; ejemplo2@correo2.com;";
            string output = string.Empty;

            var mails = lista.Split(';');
            foreach (string dir in mails)
                email.To.Add(dir);

            try {
                smtp.Send(email);
                email.Dispose();
                output = "Correo electrónico fue enviado satisfactoriamente.";
            }
            catch (SmtpException exm) {
                throw exm;
            }
            catch (Exception ex) {
                output = "Error enviando correo electrónico: " + ex.Message;
            }
            Console.WriteLine(output);
        }

    }
}
