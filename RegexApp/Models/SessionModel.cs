namespace RegexApp.Models {
    public class SessionModel {

        public string SessionID { get; set; } = "";
        public string Username { get; set; } = "";
        public DateTime CreatedAt { get; set; }
        public DateTime ExpirationTime { get; set; }
    
    }
    public enum ESessionType {
        SessionId,
        Username,
        CreatedAt,
        ExpirationTime
    }
}
