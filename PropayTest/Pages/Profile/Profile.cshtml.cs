using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PropayTest.Pages.Users;
using PropayTest.Services;
using PropayTest.Models.Question;
using PropayTest.Models.Quiz;
using System.Data.SqlClient;
using PropayTest.Models.SolvedQuiz;
using PropayTest.Models.PopularQuiz;
using PropayTest.Models.TopPerformer;
using Microsoft.AspNetCore.Http;


namespace PropayTest.Pages.Profile
{
    public class ProfileModel : PageModel
    {
        public User? User { get; set; }
        public List<Question> Questions { get; set; }

        public string errorMessage = "";
        public string successMessage = "";

        public List<int> SelectedQuestionIds { get; set; } = new List<int>();

        public List<SolvedQuiz> SolvedQuizzes { get; set; }

        public List<PopularQuiz> PopularQuizzes { get; set; }

        public List<TopPerformer> TopPerformers { get; set; }

        public void OnGet()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId != null)
            {
                // Fetch user details from the database using userId
                User = UserService.GetUserDetails((int)userId);
                Questions = QuestionService.GetAllQuestions((int)userId);
                SolvedQuizzes = QuizService.GetAllSolvedQuizzes((int)userId);
                PopularQuizzes = QuizService.GetPopularQuizzes();
                TopPerformers = UserService.GetTopPerformers();
                HttpContext.Session.SetInt32("QuestionCount", Questions.Count);
                int rank = 0;
                bool includeRank = false;
                foreach (var performer in TopPerformers)
                {
                    performer.Weight = (int)(performer.Weight * 100);
                    rank++;
                    if (performer.UserId == User.Id)
                    {
                        includeRank = true;
                        HttpContext.Session.SetInt32("Weight", (int)performer.Weight);
                        HttpContext.Session.SetInt32("QuestionCount", Questions.Count);
                        HttpContext.Session.SetInt32("Rank", rank);

                    }

                }
                if (!includeRank)
                {
                    rank = 0;
                    HttpContext.Session.SetInt32("Rank", rank);
                }
                if (Questions.Count < 5)
                {
                    errorMessage = "You need to have created at least 5 questions. You currently have " + Questions.Count;
                }
            }
        }

        public IActionResult OnPostCreateQuestion(string category, string questionText, string choiceOne, string choiceTwo, string choiceThree, string choiceFour, string answer)
        {


            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId != null)
            {
                try
                {
                    String connectionString = "Data Source=LEGION\\SQLEXPRESS;Initial Catalog=PropayTest;Integrated Security=True";
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        string sql = "INSERT INTO Questions " +
                     "(QuestionText, Choice1, Choice2, Choice3, Choice4, CorrectChoice, CreatorId, CreatedDate, HowManyAnsweredWrong, HowManyAnsweredCorrect, Category) VALUES " +
                     "(@questionText, @choice1, @choice2, @choice3, @choice4, @correctChoice, @creatorId, @createdDate, @howManyWrong, @howManyCorrect, @category)";

                        using (SqlCommand command = new SqlCommand(sql, connection))
                        {
                            // Add parameters to the command
                            command.Parameters.AddWithValue("@questionText", questionText);
                            command.Parameters.AddWithValue("@choice1", choiceOne);
                            command.Parameters.AddWithValue("@choice2", choiceTwo);
                            command.Parameters.AddWithValue("@choice3", choiceThree);
                            command.Parameters.AddWithValue("@choice4", choiceFour);
                            command.Parameters.AddWithValue("correctChoice", answer);
                            command.Parameters.AddWithValue("@creatorId", userId);
                            command.Parameters.AddWithValue("@createdDate", DateTime.Now);
                            command.Parameters.AddWithValue("@howManyWrong", 0);
                            command.Parameters.AddWithValue("@howManyCorrect", 0);
                            command.Parameters.AddWithValue("@category", category);

                            // Execute the command
                            command.ExecuteNonQuery();
                        }
                    }
                    TempData["SuccessMessage"] = "Created question!";
                    TempData["OpenSecondModal"] = "open";
                    return Redirect("/profile");

                }

                catch (Exception e)
                {
                    errorMessage = e.Message;
                    return Page();
                }
            }

            else
            {
                return Page();
            }
        }


        public IActionResult OnPostSignOut()
        {
            HttpContext.Session.Remove("UserId");
            User = null;
            TempData["SuccessMessage"] = null;
            return RedirectToPage("/Index");
        }


        public IActionResult OnPostDeleteQuestion(int SelectedQuestionId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId != null)
            {
                try
                {

                    String connectionString = "Data Source=LEGION\\SQLEXPRESS;Initial Catalog=PropayTest;Integrated Security=True";
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        string sql = "Delete from Questions where QuestionId = @QuestionId";
                        using (SqlCommand command = new SqlCommand(sql, connection))
                        {
                            // Add parameters to the command
                            command.Parameters.AddWithValue("@QuestionId", SelectedQuestionId);

                            // Execute the command
                            command.ExecuteNonQuery();
                        }
                    }



                }

                catch (Exception e)
                {
                    errorMessage = e.Message;
                }

                HttpContext.Session.SetInt32("UserId", (int)userId);
                TempData["SuccessMessage"] = "Deleted question";
                TempData["OpenSecondModal"] = "open";
                return Redirect("/profile");

            }

            else
            {
                return RedirectToPage("/Index");
            }


        }




        public IActionResult OnPostPickQuestions(string SelectedQuestionIds, string QuizTitle, string QuizDescription)
        {
            int maxQuizId = 0;
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId != null)
            {
                try
                {
                    String connectionString = "Data Source=LEGION\\SQLEXPRESS;Initial Catalog=PropayTest;Integrated Security=True";
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        string sql = "select max(Id) from Quizzes";

                        using (SqlCommand command = new SqlCommand(sql, connection))
                        {
                            maxQuizId = (int)command.ExecuteScalar();
                        }
                    }
                }

                catch (Exception e)
                {
                    errorMessage = e.Message;
                }
                try
                {
                    String connectionString = "Data Source=LEGION\\SQLEXPRESS;Initial Catalog=PropayTest;Integrated Security=True";
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        string sql = "INSERT INTO Quizzes " +
                     "(Title, Description, CreatedDate, IsActive, CreatorId) VALUES " +
                     "(@title, @description, @createdDate, @isActive, @creatorId)";

                        using (SqlCommand command = new SqlCommand(sql, connection))
                        {
                            // Add parameters to the command
                            command.Parameters.AddWithValue("@title", QuizTitle);
                            command.Parameters.AddWithValue("@description", QuizDescription);
                            command.Parameters.AddWithValue("@createdDate", DateTime.Now);
                            command.Parameters.AddWithValue("@isActive", 1);
                            command.Parameters.AddWithValue("@creatorId", userId); // Or set this value as needed

                            // Execute the command
                            command.ExecuteNonQuery();
                        }
                    }
                }

                catch (Exception e)
                {
                    errorMessage = e.Message;
                }

                // Convert the string to an integer if needed

                try
                {
                    String connectionString = "Data Source=LEGION\\SQLEXPRESS;Initial Catalog=PropayTest;Integrated Security=True";
                    string[] values = SelectedQuestionIds.Split(',');
                    foreach (string value in values)
                    {
                        int number = int.Parse(value);
                        using (SqlConnection connection = new SqlConnection(connectionString))
                        {
                            connection.Open();
                            string sql = "INSERT INTO QuizQuestions " +
                                         "(QuizId, QuestionId) VALUES " +
                                         "(@quizId, @questionId)";

                            using (SqlCommand command = new SqlCommand(sql, connection))
                            {
                                // Add parameters to the command
                                command.Parameters.AddWithValue("@quizId", maxQuizId + 1);
                                command.Parameters.AddWithValue("@questionId", number);
                                // Execute the command
                                command.ExecuteNonQuery();
                            }
                        }
                    }


                    HttpContext.Session.SetInt32("UserId", (int)userId);
                    TempData["SuccessMessage"] = "Created quiz!";
                    TempData["OpenSecondModal"] = "open";
                    return Redirect("/profile");

                }

                catch (Exception e)
                {
                    errorMessage = e.Message;
                }
                return Page();


            }

            else
            {
                return RedirectToPage("/Index");
            }


        }

    }
}
