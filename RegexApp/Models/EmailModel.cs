using System.ComponentModel.DataAnnotations;
using System.Net.Mail;
using System.Net.Mime;
using System.Net;
using System.Xml.Linq;

namespace RegexApp.Models {
    public class EmailModel {

        [Required, Display(Name = "Correo Destinatario"), EmailAddress]
        public string EmailTo { get; set; } = "";

        [Required, Display(Name = "Correo Remitente"), EmailAddress]
        public string EmailFrom { get; set; } = "";

        [Required]
        public string Subject { get; set; } = "";
        public string Body { get; set; } = "";

        [Required]
        [DataType(DataType.Password)]
        public string Pass { get; set; } = "";


        private static bool sendEmail(EmailModel model) {
            MailMessage email = new MailMessage();
            SmtpClient smtp = new SmtpClient();

            email.To.Add(new MailAddress(model.EmailTo));
            email.From = new MailAddress(model.EmailFrom);
            email.Subject = "Notificación ( " + DateTime.Now.ToString("dd / MMM / yyy hh:mm:ss") + " ) ";//model.subject
            email.SubjectEncoding = System.Text.Encoding.UTF8;
            email.Body = model.Body;
            email.IsBodyHtml = true;
            email.Priority = MailPriority.Normal;
            //FileStream fs = new FileStream("E:\\TestFolder\\test.pdf", FileMode.Open, FileAccess.Read);
            //Attachment a = new Attachment(fs, "test.pdf", MediaTypeNames.Application.Octet);
            //email.Attachments.Add(a);

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
                return false;
            }
            Console.WriteLine(output);
            return true;
        }
    }
}
