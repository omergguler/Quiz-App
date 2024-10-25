using PropayTest.Models.PopularQuiz;
using PropayTest.Models.TopPerformer;
using PropayTest.Pages.Users;
using System.Data.SqlClient;

namespace PropayTest.Services
{
    public class UserService
    {
        public static User GetUserDetails(int Id)
        {
            User user = new User();
            try
            {
                String connectionString = "Data Source=LEGION\\SQLEXPRESS;Initial Catalog=PropayTest;Integrated Security=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "SELECT * FROM Users where Id=@Id";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        // Add parameters to the command
                        command.Parameters.AddWithValue("@Id", Id);

                        // Execute the command
                        using (SqlDataReader reader = command.ExecuteReader())
                        {

                            while (reader.Read())
                            {
                                
                                user.Id = reader.GetInt32(0);
                                user.UserName = reader.GetString(1);
                                user.FullName = reader.GetString(2);
                                user.Email = reader.GetString(3);
                                user.PasswordHash = reader.GetString(4);
                                user.SecurityStamp = reader.GetString(5);
                                user.EmailConfirmed = reader.GetBoolean(6);
                                user.CreatedDate = reader.GetDateTime(7);
                            }
                        }
                    }
                    
                }

                return user;
            }

            catch (Exception e)
            {
                return user;
            }
        }

        public static List<TopPerformer> GetTopPerformers()
        {
            List<TopPerformer> topPerformers = new List<TopPerformer>();
            try
            {
                String connectionString = "Data Source=LEGION\\SQLEXPRESS;Initial Catalog=PropayTest;Integrated Security=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "SELECT \r\n    UserId, \r\n\tUsers.UserName,\r\n    SUM(HowManyCorrect) AS TotalCorrect,\r\n    SUM(HowManyWrong) AS TotalWrong,\r\n\tCAST(SUM(HowManyCorrect) AS FLOAT) / \r\n    (CAST(SUM(HowManyCorrect) AS FLOAT) + SUM(HowManyWrong)) AS Weight\r\nFROM \r\n    [dbo].[UserQuizResults]\r\n\tinner join Users on Users.Id = UserQuizResults.UserId\r\nGROUP BY \r\n    UserId, Users.UserName\r\nORDER BY\r\n\tTotalCorrect desc, Weight desc";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        
                        // Execute the command
                        using (SqlDataReader reader = command.ExecuteReader())
                        {

                            while (reader.Read())
                            {
                                TopPerformer user = new TopPerformer();
                                user.UserId = reader.GetInt32(0);
                                user.UserName = reader.GetString(1);
                                user.TotalCorrect = reader.GetInt32(2);
                                user.TotalWrong = reader.GetInt32(3);
                                user.Weight = (float)reader.GetDouble(4);
                                topPerformers.Add(user);
                                
                            }
                        }
                    }

                }

                return topPerformers;
            }

            catch (Exception e)
            {
                return topPerformers;
            }
        }
    }
}