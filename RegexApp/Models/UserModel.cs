using Microsoft.Build.Framework;
using Microsoft.Data.SqlClient;
using System.Data;
using System.ComponentModel.DataAnnotations;
using BC = BCrypt.Net.BCrypt;
using RequiredAttribute = System.ComponentModel.DataAnnotations.RequiredAttribute;
using System.Configuration;
using RegexApp.Database;

namespace RegexApp.Models {
    public class UserModel {
                                
        public int PK_Users { get; set; } = 0;
        public int FK_Users_Roles { get; set; } = 0;

        [Display(Name = "Email address")]
        [Required(ErrorMessage = "The email address is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string? Email { get; set; } = "";
        
        [Required(ErrorMessage = "Please enter your username"), MaxLength(25)]
        public string? UserName { get; set; } = "";

        [Required(ErrorMessage = "Please enter your password"), MaxLength(255)]
        public string? Password { get; set; } = "";
        public bool Enabled { get; set; } = false;

        public static Random Rnd = new Random();
        
        public static UserModel? GetUser(string email, Db db) {
            SqlDataReader? reader = null;
            UserModel? modelo = null;
            try {
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = new SqlConnection(db.ConectionString);
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

        public static bool ValidateUser(UserModel user, Db db) {
            SqlDataReader? reader = null;
            bool result = false;
            
            try {
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = new SqlConnection(db.ConectionString);
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

        public static bool CreateUser(UserModel model, Db db) {
            using (SqlCommand cmd = new SqlCommand()) {
                try {
                    cmd.Connection = new SqlConnection(db.ConectionString);
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

        public static bool UpdateUserPassword(UserModel model, Db db) {
            using SqlCommand cmd = new SqlCommand();
            using SqlConnection conn = new SqlConnection(db.ConectionString);
            try {
                cmd.Connection = conn;
                cmd.Connection.Open();

                cmd.CommandText = "UPDATE tblUsers SET Password_ = @password WHERE Email = @email AND Username = @username";
                cmd.Parameters.Add("@email", SqlDbType.VarChar, 75).Value = model.Email;
                cmd.Parameters.Add("@username", SqlDbType.VarChar, 25).Value = model.UserName;
                cmd.Parameters.Add("@password", SqlDbType.VarChar, 255).Value = model.Password;

                cmd.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex) {
                Console.WriteLine(ex);
                return false;
            }
        }

        public string GetRandomNumber() {
            return Rnd.Next(100000, 999999).ToString();
        }

    }
}
