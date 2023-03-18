using Microsoft.Data.SqlClient;

namespace RegexApp.Database {
    public class Db {

        public static string GetConectionString() {
            //Read from ENV VARIABLE or Appsettings.json
            return "Server=localhost\\SQLEXPRESS;Database=REGEX;Trusted_Connection=True;";
        }

    }
}
