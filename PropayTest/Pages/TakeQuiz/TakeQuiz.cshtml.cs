using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using PropayTest.Models.Answer;
using PropayTest.Models.Question;
using PropayTest.Models.Quiz;
using PropayTest.Pages.Users;
using PropayTest.Services;
using System.Data.SqlClient;

namespace PropayTest.Pages.TakeQuiz
{
    public class TakeQuizModel : PageModel
    {
        public User? User { get; set; }
        public Quiz? Quiz { get; set; }
        public List<Question> Questions { get; set; }

        public int correctAnswer = 0;

        public int wrongAnswer = 0;

        public string errorMessage = "";



        public int QuizId { get; set; }
        public void OnGet(int id)
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

                TempData["User"] = JsonConvert.SerializeObject(User);
                TempData["Quiz"] = JsonConvert.SerializeObject(Quiz);
                TempData["Questions"] = JsonConvert.SerializeObject(Questions);

            }
        }

        public IActionResult OnPostSubmitQuiz()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId != null)
            {
                TempData["State"] = "result";

                User = JsonConvert.DeserializeObject<User>(TempData["User"] as string);
                Quiz = JsonConvert.DeserializeObject<Quiz>(TempData["Quiz"] as string);
                Questions = JsonConvert.DeserializeObject<List<Question>>(TempData["Questions"] as string);


                string answersJson = Request.Form["answersJson"];

                var answers = JsonConvert.DeserializeObject<List<Answer>>(answersJson);
                
                foreach (var answer in answers)
                {
                    Console.WriteLine($"QuestionId: {answer.QuestionId}, Choice: {answer.Choice}");
                    foreach (var question in Questions)
                    {
                        if (question.QuestionId == int.Parse(answer.QuestionId))
                        {
                            if (question.CorrectChoice.ToString() == answer.Choice.ToString())
                            {
                                correctAnswer++;
                            }
                            else
                            {
                                wrongAnswer++;
                            }
                        }
                    }
                }

                TempData["CorrectAnswerCount"] = correctAnswer;
                TempData["WrongAnswerCount"] = wrongAnswer;

                string url = "/take-quiz?id=" + Request.Form["qid"];
                var quizId = int.Parse(Request.Form["qid"]);


                try
                {
                    String connectionString = "Data Source=LEGION\\SQLEXPRESS;Initial Catalog=PropayTest;Integrated Security=True";
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        string sql = "INSERT INTO UserQuizResults " +
                     "(QuizId, UserId, Score, CompletedDate, HowManyCorrect, HowManyWrong) VALUES " +
                     "(@quizId, @userId, @score, @completedDate, @howManyCorrect, @howManyWrong)";

                        using (SqlCommand command = new SqlCommand(sql, connection))
                        {
                            // Add parameters to the command
                            command.Parameters.AddWithValue("@quizId", quizId);
                            command.Parameters.AddWithValue("@userId", userId);
                            command.Parameters.AddWithValue("@score", "temp");
                            command.Parameters.AddWithValue("@completedDate", DateTime.Now);
                            command.Parameters.AddWithValue("@howManyCorrect", correctAnswer);
                            command.Parameters.AddWithValue("@howManyWrong", wrongAnswer);
                            // Execute the command
                            command.ExecuteNonQuery();
                        }
                    }
                }

                catch (Exception e)
                {
                    errorMessage = e.Message;
                    return Page();
                }

                return Redirect(url);
            }

            return RedirectToPage("/Index");
        }


        public IActionResult OnPostReturn()
        {
            return RedirectToPage("/browse");
        }

    }
}
