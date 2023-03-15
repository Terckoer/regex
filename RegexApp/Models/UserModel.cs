using Microsoft.Data.SqlClient;
using System.Data;
using BC = BCrypt.Net.BCrypt;

namespace RegexApp.Models {
    public class UserModel {
     
        public int PK_Users { get; set; } = 0;
        public int FK_Users_Roles { get; set; } = 0;
        public string? Email { get; set; } = "";
        public string? UserName { get; set; } = "";
        public string? Password { get; set; } = "";
        public bool Enabled { get; set; } = false;

        public static Random Rnd = new Random();
        
        public static UserModel? GetUser(string email) {
            SqlDataReader? reader = null;
            UserModel? modelo = null;
            string connection = "Server=localhost\\SQLEXPRESS;Database=REGEX;Trusted_Connection=True;";
            try {
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = new SqlConnection(connection);
                cmd.Connection.Open();
                cmd.CommandText = "SELECT PK_Users, FK_Users_Roles, Email, UserName, Password_, Enabled_ FROM tblUsers WHERE Email=@email";
                cmd.Parameters.Add("@email", SqlDbType.VarChar, 75).Value = email;

                reader = cmd.ExecuteReader();
                if (reader != null) {
                    modelo = new UserModel();
                    while (reader.Read()) {
                        modelo.PK_Users = reader.GetInt32(0);
                        modelo.FK_Users_Roles = reader.GetInt32(1);
                        modelo.Email = reader.GetString(2);
                        modelo.UserName = reader.GetString(3);
                        modelo.Password = reader.GetString(4);
                        modelo.Enabled = reader.GetBoolean(5);
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

        public static bool ValidateUser(UserModel user) {
            SqlDataReader? reader = null;
            bool result = false;
            string connection = "Server=localhost\\SQLEXPRESS;Database=REGEX;Trusted_Connection=True;";
            try {
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = new SqlConnection(connection);
                cmd.Connection.Open();
                cmd.CommandText = "SELECT UserName, Password_ FROM tblUsers WHERE UserName=@username AND Enabled_=1";
                cmd.Parameters.Add("@username", SqlDbType.VarChar, 25).Value = user.UserName;

                reader = cmd.ExecuteReader();
                if (reader != null && reader.Read()) {
                    result = BC.Verify(user.Password, reader.GetString(1));
                }
            }
            catch (Exception ex) {
                Console.WriteLine(ex);
            }
            finally {
                if (reader != null && !reader.IsClosed)
                    reader.Close();
            }
            return result;
        }

        public static bool CreateUser(UserModel model) {
            using (SqlCommand cmd = new SqlCommand()) {
                try {
                    cmd.Connection = new SqlConnection("Server=localhost\\SQLEXPRESS;Database=REGEX;Trusted_Connection=True;");
                    cmd.Connection.Open();

                    cmd.CommandText = "INSERT INTO tblUsers (FK_Users_Roles, Email, UserName, Password_, Enabled_) VALUES (@fkUserRole, @email, @username, @password, @enabled)";
                    cmd.Parameters.Add("@fkUserRole", SqlDbType.Int).Value = model.FK_Users_Roles;
                    cmd.Parameters.Add("@email", SqlDbType.VarChar, 75).Value = model.Email;
                    cmd.Parameters.Add("@username", SqlDbType.VarChar, 25).Value = model.UserName;
                    cmd.Parameters.Add("@password", SqlDbType.VarChar, 255).Value = model.Password;
                    cmd.Parameters.Add("@enabled", SqlDbType.Bit).Value = model.Enabled;
                    cmd.ExecuteNonQuery();
                    return true;
                }
                catch (Exception ex) {
                    Console.WriteLine(ex);
                    return false;
                }
            }
        }

        public string GetRandomNumber() {
            return Rnd.Next(100000, 999999).ToString();
        }

    }
}
