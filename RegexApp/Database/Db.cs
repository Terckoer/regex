
namespace RegexApp.Database {
    public class Db {

        public string ConectionString { get; }
        public Db(string ConectionString) {
            this.ConectionString = ConectionString;
        }

        //public string GetConectionString() {
        //    ////Read from ENV VARIABLE or Appsettings.json
        //    //"Server=localhost\\SQLEXPRESS;Database=REGEX;Trusted_Connection=True;";
        //    return configuration.GetConnectionString("conn");
        //}

    }
}
