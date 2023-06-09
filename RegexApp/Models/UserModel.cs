﻿using Microsoft.Build.Framework;
using Microsoft.Data.SqlClient;
using System.Data;
using System.ComponentModel.DataAnnotations;
using BC = BCrypt.Net.BCrypt;
using RequiredAttribute = System.ComponentModel.DataAnnotations.RequiredAttribute;
using System.Configuration;
using RegexApp.Database;
using Microsoft.DotNet.Scaffolding.Shared.Project;

namespace RegexApp.Models {
    public class UserModel {
                                
        public int PK_Users { get; set; } = 0;
        public int FK_Users_Roles { get; set; } = 0;

        [Required(ErrorMessage = "Please enter your username"), MaxLength(256)]
        public string? Username { get; set; } = "";

        [Required(ErrorMessage = "Please enter your password")]
        public string? Password { get; set; } = "";

        [Display(Name = "Email address")]
        [Required(ErrorMessage = "The email address is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string? Email { get; set; } = "";
        public bool EmailConfirmed { get; set; } = false;
        public string PhoneNumber { get; set; } = "";
        public bool PhoneNumberConfirmed { get; set; } = false;
        public bool Enabled { get; set; } = false;
        public bool TwoFactorEnabled { get; set; } = false;
        public string SecurityStamp { get; set; } = "";
        public DateTime LockoutEndDateUtc { get; set; }
        public bool LockoutEnabled { get; set; } = false;
        public int AccessFailedCount { get; set; }

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
                        modelo.Username = reader.GetString(3);
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

        public static UserModel? GetUserWithActiveToken(Guid token, Db db) {
            SqlDataReader? reader = null;
            UserModel? modelo = null;
            try {
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = new SqlConnection(db.ConectionString);
                cmd.Connection.Open();
                cmd.CommandText = "SELECT u.PK_Users, u.Email, u.UserName " +
                                  "FROM tblUsers u " +
                                  "INNER JOIN tblTempTokens tk ON u.PK_Users = tk.FK_TempTokens_Users " +
                                  "WHERE tk.Token = @token AND tk.Expiration_Date > GETDATE() AND tk.Enabled_ = 1";
                cmd.Parameters.Add("@token", SqlDbType.UniqueIdentifier).Value = token;

                reader = cmd.ExecuteReader();
                if (reader != null) {
                    modelo = new UserModel();
                    while (reader.Read()) {
                        modelo.PK_Users = reader.GetInt32(0);
                        modelo.Email = reader.GetString(1);
                        modelo.Username = reader.GetString(2);
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

        public static bool ExistUser(UserModel user, Db db) {
            SqlDataReader? reader = null;
            bool result = false;

            try {
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = new SqlConnection(db.ConectionString);
                cmd.Connection.Open();
                cmd.CommandText = "SELECT UserName FROM tblUsers WHERE UserName=@username";
                cmd.Parameters.Add("@username", SqlDbType.NVarChar, 256).Value = user.Username;

                reader = cmd.ExecuteReader();
                if (reader != null && reader.Read()) 
                    result = reader.GetString(0) == user.Username;
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

        public static bool ValidateUser(UserModel user, Db db) {
            SqlDataReader? reader = null;
            bool result = false;
            
            try {
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = new SqlConnection(db.ConectionString);
                cmd.Connection.Open();
                cmd.CommandText = "SELECT UserName, Password_ FROM tblUsers WHERE UserName=@username AND Enabled_=1";
                cmd.Parameters.Add("@username", SqlDbType.NVarChar, 256).Value = user.Username;

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

                    cmd.CommandText = "INSERT INTO " +
                                      "tblUsers (FK_Users_Roles, Email, UserName, Password_, Enabled_, Email_Confirmed, Phone_Number_Confirmed, Two_Factor_Enabled, Lockout_Enabled, Access_Failed_Count) " +
                                      "VALUES (@fkUserRole, @email, @username, @password, @enabled, @emailConfirmed, @phoneNumberConfirmed, @twoFactorEnabled, @lockoutEnabled, @accessFailedCount)";
                    cmd.Parameters.Add("@fkUserRole", SqlDbType.Int).Value = model.FK_Users_Roles;
                    cmd.Parameters.Add("@email", SqlDbType.NVarChar, 256).Value = model.Email;
                    cmd.Parameters.Add("@username", SqlDbType.NVarChar, 256).Value = model.Username;
                    cmd.Parameters.Add("@password", SqlDbType.NVarChar).Value = model.Password;
                    cmd.Parameters.Add("@enabled", SqlDbType.Bit).Value = model.Enabled;
                    cmd.Parameters.Add("@emailConfirmed", SqlDbType.Bit).Value = model.EmailConfirmed;
                    cmd.Parameters.Add("@phoneNumberConfirmed", SqlDbType.Bit).Value = model.PhoneNumberConfirmed;
                    cmd.Parameters.Add("@twoFactorEnabled", SqlDbType.Bit).Value = model.TwoFactorEnabled;
                    cmd.Parameters.Add("@lockoutEnabled", SqlDbType.Bit).Value = model.LockoutEnabled;
                    cmd.Parameters.Add("@accessFailedCount", SqlDbType.Int).Value = model.AccessFailedCount;


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
                cmd.Parameters.Add("@email", SqlDbType.NVarChar, 256).Value = model.Email;
                cmd.Parameters.Add("@username", SqlDbType.NVarChar, 256).Value = model.Username;
                cmd.Parameters.Add("@password", SqlDbType.NVarChar).Value = model.Password;
                return cmd.ExecuteNonQuery()>0;
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
