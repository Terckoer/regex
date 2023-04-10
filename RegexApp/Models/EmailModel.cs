namespace RegexApp.Models {
    public class EmailModel {
        public string Email { get; set; } = "";
        public string Subject { get; set; } = "";
        public string Body { get; set; } = "";

        public static bool SendEmail(EmailModel modelo) {
            //SEND THE EMAIL
            
            return true;
        }
    }
}
