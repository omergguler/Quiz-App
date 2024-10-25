using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PropayTest.Models.Question;
using PropayTest.Models.Quiz;
using PropayTest.Pages.Users;
using PropayTest.Services;
using System.Data.SqlClient;

namespace PropayTest.Pages.MyPerformance
{
    public class MyPerformanceModel : PageModel
    {
        public User? User { get; set; }

        public List<Quiz> Quizzes { get; set; }

        public List<Question> Questions { get; set; }

        public int questionCount { get; set; }

        public int quizCount { get; set; }

        public int rank { get; set; }
        public int weight { get; set; }
        public string quizCreationMsg { get; set; }
        public string quizCreationImg { get; set; }
        public string questionCreationMsg { get; set; }
        public string questionCreationImg { get; set; }
        public string quizzesGotSolvedMsg { get; set; }
        public string quizzesGotSolvedImg { get; set; }
        public string quizzesSolvedMsg { get; set; }
        public string quizzesSolvedImg { get; set; }
        public void OnGet()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId != null)
            {
                User = UserService.GetUserDetails((int)userId);
                Quizzes = QuizService.GetAllQuizzes((int)userId);
                Questions = QuestionService.GetAllQuestions((int)userId);
                checkQuizCreationPerformance((int)userId);
                checkQuestionCreationPerformance((int)userId);
                checkQuizGottenSolvedPerformance((int)userId);
                checkQuizSolvedPerformance((int)userId);
            }
        }

        private void checkQuizCreationPerformance(int userId)
        {
            int currentPeriodCount = 0;
            int previousPeriodCount = 0;
            try
            {
                String connectionString = "Data Source=LEGION\\SQLEXPRESS;Initial Catalog=PropayTest;Integrated Security=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "DECLARE @CurrentPeriodStart DATE = GETDATE() - 7;\r\nDECLARE @PreviousPeriodStart DATE = GETDATE() - 14;\r\nDECLARE @PreviousPeriodEnd DATE = GETDATE() - 7;\r\n\r\n-- Count the quizzes for the current and previous periods\r\nSELECT \r\n    SUM(CASE WHEN CreatedDate >= @CurrentPeriodStart THEN 1 ELSE 0 END) AS CurrentPeriodCount,\r\n    SUM(CASE WHEN CreatedDate >= @PreviousPeriodStart AND CreatedDate < @PreviousPeriodEnd THEN 1 ELSE 0 END) AS PreviousPeriodCount\r\nFROM Quizzes\r\nWHERE CreatorId = @userId";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        // Add parameters to the command
                        command.Parameters.AddWithValue("@userId", userId);

                        // Execute the command
                        using (SqlDataReader reader = command.ExecuteReader())
                        {

                            while (reader.Read())
                            {
                                currentPeriodCount = reader.GetInt32(0);
                                previousPeriodCount = reader.GetInt32(1);
                            }
                        }
                    }
                }

            }

            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            double percentageChange;
            quizCreationImg = "";
            if (previousPeriodCount == 0)
            {
                if (currentPeriodCount > 0)
                {
                    percentageChange = 100.0;
                    quizCreationMsg = "Your quiz creation is up 100% compared to past week.";
                    quizCreationImg = "/icons/up.gif";
                }
                else
                {
                    percentageChange = 0.0;
                    quizCreationMsg = "Compared to last week, there is no change in your quiz creation.";
                }
            }
            else
            {
                percentageChange = ((double)(currentPeriodCount - previousPeriodCount) / previousPeriodCount) * 100;
            }

            if (percentageChange < 0)
            {
                quizCreationMsg = $"Your quiz creation is down {Math.Abs(percentageChange):F2}% compared to past week.";
                quizCreationImg = "/icons/down.gif";
            }
            else if (percentageChange == 0)
            {
                quizCreationMsg = "Compared to last week, there is no change in your quiz creation.";
            } else
            {
                quizCreationMsg = $"Your quiz creation is up {percentageChange}% compared to past week.";
                quizCreationImg = "/icons/up.gif";
            }

        }

        private void checkQuestionCreationPerformance(int userId)
        {
            int currentPeriodCount = 0;
            int previousPeriodCount = 0;
            try
            {
                String connectionString = "Data Source=LEGION\\SQLEXPRESS;Initial Catalog=PropayTest;Integrated Security=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "DECLARE @CurrentPeriodStart DATE = GETDATE() - 7;\r\nDECLARE @PreviousPeriodStart DATE = GETDATE() - 14;\r\nDECLARE @PreviousPeriodEnd DATE = GETDATE() - 7;\r\n\r\n-- Count the quizzes for the current and previous periods\r\nSELECT \r\n    SUM(CASE WHEN CreatedDate >= @CurrentPeriodStart THEN 1 ELSE 0 END) AS CurrentPeriodCount,\r\n    SUM(CASE WHEN CreatedDate >= @PreviousPeriodStart AND CreatedDate < @PreviousPeriodEnd THEN 1 ELSE 0 END) AS PreviousPeriodCount\r\nFROM Questions\r\nWHERE CreatorId = @userId";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        // Add parameters to the command
                        command.Parameters.AddWithValue("@userId", userId);

                        // Execute the command
                        using (SqlDataReader reader = command.ExecuteReader())
                        {

                            while (reader.Read())
                            {
                                currentPeriodCount = reader.GetInt32(0);
                                previousPeriodCount = reader.GetInt32(1);
                            }
                        }
                    }
                }

            }

            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            double percentageChange;
            questionCreationImg = "";
            if (previousPeriodCount == 0)
            {
                if (currentPeriodCount > 0)
                {
                    percentageChange = 100.0;
                    questionCreationMsg = "Your question creation is up 100% compared to past week.";
                    questionCreationImg = "/icons/up.gif";
                }
                else
                {
                    percentageChange = 0.0;
                    questionCreationMsg = "Compared to last week, there is no change in your question creation.";
                }
            }
            else
            {
                percentageChange = ((double)(currentPeriodCount - previousPeriodCount) / previousPeriodCount) * 100;
            }

            if (percentageChange < 0)
            {
                questionCreationMsg = $"Your question creation is down {Math.Abs(percentageChange):F2}% compared to past week.";
                questionCreationImg = "/icons/down.gif";
            }
            else if (percentageChange == 0)
            {
                questionCreationMsg = "Compared to last week, there is no change in your question creation.";
            }
            else
            {
                questionCreationMsg = $"Your question creation is up {percentageChange}% compared to past week.";
                questionCreationImg = "/icons/up.gif";
            }

        }

        private void checkQuizGottenSolvedPerformance(int userId)
        {
            int currentPeriodCount = 0;
            int previousPeriodCount = 0;
            try
            {
                String connectionString = "Data Source=LEGION\\SQLEXPRESS;Initial Catalog=PropayTest;Integrated Security=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "select \r\n    count(case when DATEDIFF(day, UserQuizResults.CompletedDate, GETDATE()) <= 7 then 1 end) as Last7DaysCount,\r\n    count(case when DATEDIFF(day, UserQuizResults.CompletedDate, GETDATE()) > 7 and DATEDIFF(day, UserQuizResults.CompletedDate, GETDATE()) <= 14 then 1 end) as Last7to14DaysCount\r\nfrom UserQuizResults\r\ninner join Quizzes on Quizzes.Id = UserQuizResults.QuizId\r\nwhere Quizzes.CreatorId = @userId";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        // Add parameters to the command
                        command.Parameters.AddWithValue("@userId", userId);

                        // Execute the command
                        using (SqlDataReader reader = command.ExecuteReader())
                        {

                            while (reader.Read())
                            {
                                currentPeriodCount = reader.GetInt32(0);
                                previousPeriodCount = reader.GetInt32(1);
                            }
                        }
                    }
                }

            }

            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            double percentageChange;
            quizzesGotSolvedMsg = "";
            if (previousPeriodCount == 0)
            {
                if (currentPeriodCount > 0)
                {
                    percentageChange = 100.0;
                    quizzesGotSolvedMsg = "Your quizzes are taken 100% more compared to past week.";
                    quizzesGotSolvedImg = "/icons/up.gif";
                }
                else
                {
                    percentageChange = 0.0;
                    quizzesGotSolvedMsg = "Compared to last week, your quizzes were taken same amount of times.";
                }
            }
            else
            {
                percentageChange = ((double)(currentPeriodCount - previousPeriodCount) / previousPeriodCount) * 100;
            }

            if (percentageChange < 0)
            {
                quizzesGotSolvedMsg = $"Your quizzes were taken {Math.Abs(percentageChange):F2}% less compared to past week.";
                quizzesGotSolvedImg = "/icons/down.gif";
            }
            else if (percentageChange == 0)
            {
                quizzesGotSolvedMsg = "Compared to last week, your quizzes were taken same amount of times.";
            }
            else
            {
                quizzesGotSolvedMsg = $"Your quizzes were taken {percentageChange}% more compared to past week.";
                quizzesGotSolvedImg = "/icons/up.gif";
            }

        }

        private void checkQuizSolvedPerformance(int userId)
        {
            int currentPeriodCount = 0;
            int previousPeriodCount = 0;
            try
            {
                String connectionString = "Data Source=LEGION\\SQLEXPRESS;Initial Catalog=PropayTest;Integrated Security=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "select \r\n    count(case when DATEDIFF(day, UserQuizResults.CompletedDate, GETDATE()) <= 7 then 1 end) as Last7DaysCount,\r\n    count(case when DATEDIFF(day, UserQuizResults.CompletedDate, GETDATE()) > 7 and DATEDIFF(day, UserQuizResults.CompletedDate, GETDATE()) <= 14 then 1 end) as Last7to14DaysCount\r\nfrom UserQuizResults\r\nwhere UserQuizResults.UserId = @userId";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        // Add parameters to the command
                        command.Parameters.AddWithValue("@userId", userId);

                        // Execute the command
                        using (SqlDataReader reader = command.ExecuteReader())
                        {

                            while (reader.Read())
                            {
                                currentPeriodCount = reader.GetInt32(0);
                                previousPeriodCount = reader.GetInt32(1);
                            }
                        }
                    }
                }

            }

            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            double percentageChange;
            quizzesSolvedMsg = "";
            if (previousPeriodCount == 0)
            {
                if (currentPeriodCount > 0)
                {
                    percentageChange = 100.0;
                    quizzesSolvedMsg = "You solved 100% more quizzes compared to past week.";
                    quizzesSolvedImg = "/icons/up.gif";
                }
                else
                {
                    percentageChange = 0.0;
                    quizzesSolvedMsg = "Compared to last week, you solved same amount of quizzes.";
                }
            }
            else
            {
                percentageChange = ((double)(currentPeriodCount - previousPeriodCount) / previousPeriodCount) * 100;
            }

            if (percentageChange < 0)
            {
                quizzesSolvedMsg = $"You solved {Math.Abs(percentageChange):F2}% less quizzes compared to past week.";
                quizzesSolvedImg = "/icons/down.gif";
            }
            else if (percentageChange == 0)
            {
                quizzesSolvedMsg = "Compared to last week, you solved same amount of quizzes.";
            }
            else
            {
                quizzesSolvedMsg = $"You solved {percentageChange}% more quizzes compared to past week.";
                quizzesSolvedImg = "/icons/up.gif";
            }

        }
    }
}
