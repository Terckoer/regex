namespace RegexApp.Models {
    public class SessionModel {

        public string SessionID { get; set; } = "";
        public string UserID { get; set; } = "";
        public DateTime CreatedAt { get; set; }
        public DateTime ExpirationTime { get; set; }
    
    }
}
