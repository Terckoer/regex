using Microsoft.Data.SqlClient;
using System.Data;

namespace RegexApp.Models {
    public class UserModel {
     
        public int PK_Users { get; set; } = 0;
        public int FK_Users_Roles { get; set; } = 0;
        public string? Email { get; set; } = "";
        public string? UserName { get; set; } = "";
        public string? Password { get; set; } = "";
        public bool Enabled { get; set; } = false;

        public static Random rnd = new Random();
        
        public static UserModel? GetUser(string email) {
            SqlDataReader? reader = null;
            UserModel modelo = null;
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

        public string GetRandomNumber() {
            return rnd.Next(100000, 999999).ToString();
        }

    }
}
