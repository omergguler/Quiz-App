using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PropayTest.Models.Question;
using PropayTest.Models.Quiz;
using PropayTest.Pages.Users;
using PropayTest.Services;
using System.Data.SqlClient;

namespace PropayTest.Pages.QuizPage
{
    public class QuizPageModel : PageModel
    {
        public User? User { get; set; }

        public Quiz? Quiz { get; set; }
        public int QuizId { get; set; }
        public List<Question> Questions { get; set; }


        public void OnGet(int id, string title)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId != null)
            {
                User = UserService.GetUserDetails((int)userId);
                QuizId = id;
                Questions = new List<Question>();

                try
                {
                    String connectionString = "Data Source=LEGION\\SQLEXPRESS;Initial Catalog=PropayTest;Integrated Security=True";
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        string sql = "SELECT * FROM Quizzes where Id=@Id";

                        using (SqlCommand command = new SqlCommand(sql, connection))
                        {
                            // Add parameters to the command
                            command.Parameters.AddWithValue("@Id", QuizId);

                            // Execute the command
                            using (SqlDataReader reader = command.ExecuteReader())
                            {

                                while (reader.Read())
                                {
                                    Quiz = new Quiz();
                                    Quiz.Id = reader.GetInt32(0);
                                    Quiz.Title = reader.GetString(1);
                                    Quiz.Description = reader.GetString(2);
                                    Quiz.CreatedDate = reader.GetDateTime(3);
                                    Quiz.IsActive = reader.GetBoolean(4);
                                    Quiz.CreatorId = reader.GetInt32(5);
                                }
                            }
                        }
                    }

                }

                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

                try
                {
                    String connectionString = "Data Source=LEGION\\SQLEXPRESS;Initial Catalog=PropayTest;Integrated Security=True";
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        string sql = "SELECT * FROM Questions inner join QuizQuestions on Questions.QuestionId = QuizQuestions.QuestionId where QuizQuestions.QuizId = @Id";

                        using (SqlCommand command = new SqlCommand(sql, connection))
                        {
                            // Add parameters to the command
                            command.Parameters.AddWithValue("@Id", QuizId);

                            // Execute the command
                            using (SqlDataReader reader = command.ExecuteReader())
                            {

                                while (reader.Read())
                                {
                                    Question Question = new Question();
                                    Question.QuestionId = reader.GetInt32(0);
                                    Question.QuestionText = reader.GetString(1);
                                    Question.Choice1 = reader.GetString(2);
                                    Question.Choice2 = reader.GetString(3);
                                    Question.Choice3 = reader.GetString(4);
                                    Question.Choice4 = reader.GetString(5);
                                    Question.CorrectChoice = reader.GetString(6);
                                    Question.CreatorId = reader.GetInt32(7);
                                    Question.CreatedDate = reader.GetDateTime(8);
                                    Question.HowManyAnsweredWrong = reader.GetInt32(9);
                                    Question.HowManyAnsweredCorrect = reader.GetInt32(10);
                                    Question.Category = reader.GetString(11);
                                    Questions.Add(Question);
                                }
                            }
                        }
                    }

                }

                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }
    }
}
