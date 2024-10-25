using PropayTest.Models.Question;
using PropayTest.Pages.Users;
using System.Data.SqlClient;

namespace PropayTest.Services
{
    public class QuestionService
    {


        public static List<Question> GetAllQuestions(int UserId)
        {
            List<Question> questions = new List<Question>();
            try
            {
                String connectionString = "Data Source=LEGION\\SQLEXPRESS;Initial Catalog=PropayTest;Integrated Security=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "SELECT * FROM Questions where CreatorId=@creatorId order by CreatedDate desc";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        // Add parameters to the command
                        command.Parameters.AddWithValue("@creatorId", UserId);

                        // Execute the command
                        using (SqlDataReader reader = command.ExecuteReader())
                        {

                            while (reader.Read())
                            {
                                Question question = new Question();
                                question.QuestionId = reader.GetInt32(0);
                                question.QuestionText = reader.GetString(1);
                                question.Choice1 = reader.GetString(2);
                                question.Choice2 = reader.GetString(3);
                                question.Choice3 = reader.GetString(4);
                                question.Choice4 = reader.GetString(5);
                                question.CorrectChoice = reader.GetString(6);
                                question.CreatorId = reader.GetInt32(7);
                                question.CreatedDate = reader.GetDateTime(8);
                                question.HowManyAnsweredWrong = reader.GetInt32(9);
                                question.HowManyAnsweredCorrect = reader.GetInt32(10);
                                question.Category = reader.GetString(11);
                                questions.Add(question);
                            }
                        }
                    }
                }

            }

            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            
            return questions;
        }

    }
}
