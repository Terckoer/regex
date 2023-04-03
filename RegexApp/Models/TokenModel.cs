using Microsoft.Data.SqlClient;
using RegexApp.Database;
using System.Data;

namespace RegexApp.Models {
    public class TokenModel {
        public int PkTempToken { get; set; }
        public int FkTempTokenUsers { get; set; }
        public string Token { get; set; } = "";
        public DateTime ExpirationDate { get; set; }
        public DateTime CreationDate { get; set; }

        public static TokenModel? GetToken(int pkUser, Db db) {
            SqlDataReader? reader = null;
            TokenModel? modelo = null;
            try {
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = new SqlConnection(db.ConectionString);
                cmd.Connection.Open();
                cmd.CommandText = "SELECT PK_Temp_Token, FK_TempTokens_Users, Token, Creation_Date, Expiration_Date " +
                                  "FROM tblTempTokens " +
                                  "WHERE FK_TempTokens_Users = @user";
                cmd.Parameters.Add("@user", SqlDbType.Int).Value = pkUser;

                reader = cmd.ExecuteReader();
                if (reader != null) {
                    modelo = new TokenModel();
                    while (reader.Read()) {
                        modelo.PkTempToken = reader.GetInt32(0);
                        modelo.FkTempTokenUsers = reader.GetInt32(1);
                        modelo.Token= reader.GetString(2);
                        modelo.CreationDate= reader.GetDateTime(3);
                        modelo.ExpirationDate= reader.GetDateTime(4);
                    }
                }
            }
            catch (Exception ex) {
                Console.WriteLine(ex);
            }
            finally {
                if (reader != null && !reader.IsClosed)
                    reader.Close();
            }
            return modelo;
        }

        public static bool AddToken(int pkUser, Db db) {
            using (SqlCommand cmd = new SqlCommand()) 
            try {
                cmd.Connection = new SqlConnection(db.ConectionString);
                cmd.Connection.Open();
                cmd.CommandText = "INSERT INTO tblTempTokens(FK_TempTokens_Users) VALUES(@user)";
                cmd.Parameters.Add("@user", SqlDbType.Int).Value = pkUser;

                cmd.ExecuteNonQuery();
            }
            catch (Exception ex) {
                Console.WriteLine(ex);
            }
            return true;
        }
    }
}
