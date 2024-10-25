using System.Text;
using System.Security.Cryptography;
using System;
using System.Xml.Serialization;
using PropayTest.Pages.Users;
using System.Data.SqlClient;
namespace PropayTest.Services
{
    public class UserUniquenessChecker
    {
        //    this class checks:
        //    if user enters unique email and username when creating a new account
        //    if user enters their correct information when logging in

        public static bool isUniqueUser(string userName, string email)
        {

            // look for users in the database with the usernames/emails in the parameter, and return true if there isn't any.
            List<User> usersList = new List<User>();
            
            // first check the user name.
            try
            {
                String connectionString = "Data Source=LEGION\\SQLEXPRESS;Initial Catalog=PropayTest;Integrated Security=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "SELECT * FROM Users where UserName=@inputUserName";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        // Add parameters to the command
                        command.Parameters.AddWithValue("@inputUserName", userName);

                        // Execute the command
                        using (SqlDataReader reader = command.ExecuteReader())
                        {

                            while (reader.Read())
                            {
                                User userInfo = new User();
                                userInfo.Id = reader.GetInt32(0);
                                userInfo.UserName = reader.GetString(1);
                                userInfo.FullName = reader.GetString(2);
                                userInfo.Email = reader.GetString(3);
                                userInfo.PasswordHash = reader.GetString(4);
                                userInfo.SecurityStamp = reader.GetString(5);
                                userInfo.EmailConfirmed = reader.GetBoolean(6);
                                userInfo.CreatedDate = reader.GetDateTime(7);
                                usersList.Add(userInfo);
                            }
                        }
                    }
                }

            }

            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            if (usersList.Count != 0) // means there is someone with the specified user name, so we return false directly
            {
                return false;
            } else // there is nobody with the specified user name, we check email now.
            {
                try
                {
                    String connectionString = "Data Source=LEGION\\SQLEXPRESS;Initial Catalog=PropayTest;Integrated Security=True";
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        string sql = "SELECT * FROM Users where email=@inputEmail";

                        using (SqlCommand command = new SqlCommand(sql, connection))
                        {
                            // Add parameters to the command
                            command.Parameters.AddWithValue("@inputEmail", email);

                            // Execute the command
                            using (SqlDataReader reader = command.ExecuteReader())
                            {

                                while (reader.Read())
                                {
                                    User userInfo = new User();
                                    userInfo.Id = reader.GetInt32(0);
                                    userInfo.UserName = reader.GetString(1);
                                    userInfo.FullName = reader.GetString(2);
                                    userInfo.Email = reader.GetString(3);
                                    userInfo.PasswordHash = reader.GetString(4);
                                    userInfo.SecurityStamp = reader.GetString(5);
                                    userInfo.EmailConfirmed = reader.GetBoolean(6);
                                    userInfo.CreatedDate = reader.GetDateTime(7);
                                    usersList.Add(userInfo);
                                }
                            }
                        }
                    }

                }

                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

                if (usersList.Count != 0)
                {
                    return false; // the email is not unique
                } else 
                {
                    return true; // both username and email are unique.
                }
            }





            
        }
    }
}
