using Microsoft.Data.SqlClient;
using RegexApp.Database;
using System.Data;

namespace RegexApp.Models {
    public class TokenModel {
        public int PkTempToken { get; set; }
        public int FkTempTokenUsers { get; set; }
        public Guid Token { get; set; }
        public DateTime ExpirationDate { get; set; }
        public DateTime CreationDate { get; set; }
        public bool IsEnabled { get; set; }

        public static TokenModel? GetTokenByUser(int pkUser, Db db) {
            SqlDataReader? reader = null;
            TokenModel? modelo = null;
            try {
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = new SqlConnection(db.ConectionString);
                cmd.Connection.Open();
                cmd.CommandText = "SELECT TOP 1 PK_Temp_Token, FK_TempTokens_Users, Token, Creation_Date, Expiration_Date, Enabled_ " +
                                  "FROM tblTempTokens " +
                                  "WHERE FK_TempTokens_Users = @user AND Expiration_Date > GETDATE() AND Enabled_ = 1" +
                                  "ORDER BY Expiration_Date DESC";
                cmd.Parameters.Add("@user", SqlDbType.Int).Value = pkUser;

                reader = cmd.ExecuteReader();
                if (reader != null) {
                    while (reader.Read()) {
                        modelo = new TokenModel();
                        modelo.PkTempToken = reader.GetInt32(0);
                        modelo.FkTempTokenUsers = reader.GetInt32(1);
                        modelo.Token = reader.GetGuid(2);
                        modelo.CreationDate= reader.GetDateTime(3);
                        modelo.ExpirationDate= reader.GetDateTime(4);
                        modelo.IsEnabled= reader.GetBoolean(5);

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

        public static TokenModel? GetToken(Guid token, Db db) {
            SqlDataReader? reader = null;
            TokenModel? modelo = null;
            try {
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = new SqlConnection(db.ConectionString);
                cmd.Connection.Open();
                cmd.CommandText = "SELECT TOP 1 PK_Temp_Token, FK_TempTokens_Users, Token, Creation_Date, Expiration_Date, Enabled_ " +
                                  "FROM tblTempTokens " +
                                  "WHERE Token = @token AND Expiration_Date > GETDATE() AND Enabled_ = 1" +
                                  "ORDER BY Expiration_Date DESC";
                cmd.Parameters.Add("@token", SqlDbType.UniqueIdentifier).Value = token;

                reader = cmd.ExecuteReader();
                if (reader != null) {
                    while (reader.Read()) {
                        modelo = new TokenModel();
                        modelo.PkTempToken = reader.GetInt32(0);
                        modelo.FkTempTokenUsers = reader.GetInt32(1);
                        modelo.Token = reader.GetGuid(2);
                        modelo.CreationDate = reader.GetDateTime(3);
                        modelo.ExpirationDate = reader.GetDateTime(4);
                        modelo.IsEnabled = reader.GetBoolean(5);
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
            int result = 0;
            using (SqlCommand cmd = new SqlCommand()) 
            try {
                cmd.Connection = new SqlConnection(db.ConectionString);
                cmd.Connection.Open();
                cmd.CommandText = "INSERT INTO tblTempTokens(FK_TempTokens_Users) VALUES(@user)";
                cmd.Parameters.Add("@user", SqlDbType.Int).Value = pkUser;

                result=cmd.ExecuteNonQuery();
            }
            catch (Exception ex) {
                Console.WriteLine(ex);
            }
            return result > 0;
        }

        public static bool DisableToken(Guid token, Db db) {
            try {
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = new SqlConnection(db.ConectionString);
                cmd.Connection.Open();
                cmd.CommandText = "UPDATE tblTempTokens SET Enabled_ = 0 WHERE Token = @token";
                cmd.Parameters.Add("@token", SqlDbType.UniqueIdentifier).Value = token;
                return cmd.ExecuteNonQuery() > 0;
            }
            catch (Exception ex) {
                Console.WriteLine(ex);
                return false;
            }
        }

    }
}
