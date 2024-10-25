using PropayTest.Pages.Users;
using System.Data.SqlClient;
using System.Security.Cryptography.X509Certificates;

namespace PropayTest.Services
{
    public class UserCredentialsValidator
    {
        public static User validAndConfirmed(string userNameOrEmail)
        {
            User user = null;
            
            try
            {
                String connectionString = "Data Source=LEGION\\SQLEXPRESS;Initial Catalog=PropayTest;Integrated Security=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "SELECT * FROM Users where UserName = @userNameOrEmail or Email = @userNameOrEmail";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        // Add parameters to the command
                        command.Parameters.AddWithValue("@userNameOrEmail", userNameOrEmail);
                        
                        // Execute the command
                        using (SqlDataReader reader = command.ExecuteReader())
                        {

                            if (reader.Read())
                            {
                                user = new User
                                {
                                    Id = reader.GetInt32(0),
                                    UserName = reader.GetString(1),
                                    FullName = reader.GetString(2),
                                    Email = reader.GetString(3),
                                    PasswordHash = reader.GetString(4),
                                    SecurityStamp = reader.GetString(5),
                                    EmailConfirmed = reader.GetBoolean(6),
                                    CreatedDate = reader.GetDateTime(7),
                                    SixDigitNumber = reader.GetString(8)
                                };
                            }
                        }
                    }
                }
                return user;  // Return the found user or an empty user object if none is found
            }

            catch (Exception e)
            {
                Console.WriteLine("Error occurred: " + e.Message);
                return new User();  // Return an empty user object in case of error
            }

            
        }
    }
}
