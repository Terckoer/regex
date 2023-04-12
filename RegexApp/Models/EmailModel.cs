using System.ComponentModel.DataAnnotations;
using System.Net.Mail;
using System.Net.Mime;
using System.Net;
using System.Xml.Linq;

namespace RegexApp.Models {
    public class EmailModel {

        [Required, Display(Name = "Correo Destinatario"), EmailAddress]
        public string EmailTo { get; set; } = "";        
        [Required]
        public string Subject { get; set; } = "";
        public string Body { get; set; } = "";
                

        public static bool SendEmail(EmailModel model, IConfiguration configuration) {
            MailMessage email = new MailMessage();
            SmtpClient smtp = new SmtpClient();

            email.To.Add(new MailAddress(model.EmailTo));
            email.From = new MailAddress(configuration.GetValue<string>("Smtp:email"));
            email.Subject = "Notificación ( " + DateTime.Now.ToString("dd / MMM / yyy hh:mm:ss") + " ) ";//model.subject
            email.SubjectEncoding = System.Text.Encoding.UTF8;
            email.Body = model.Body;
            email.IsBodyHtml = true;
            email.Priority = MailPriority.Normal;
            
            smtp.Host = configuration.GetValue<string>("Smtp:host");  // IP empresa/institucional
                                                                      //smtp.Host = "smtp.hotmail.com";
                                                                      //smtp.Host = "smtp.gmail.com";
            smtp.Port = configuration.GetValue<int>("Smtp:PORT");
            smtp.Timeout = 100000;
            smtp.EnableSsl = true;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new NetworkCredential(configuration.GetValue<string>("Smtp:emailAuth"), configuration.GetValue<string>("Smtp:passwordAuth"));
            

            //string lista = "jorgejs.tech@gmail.com;";
            string output = "";

            //var mails = lista.Split(';');
            //foreach (string dir in mails)
            //    email.To.Add(dir);

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
