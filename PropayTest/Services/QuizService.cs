using PropayTest.Models.PopularQuiz;
using PropayTest.Models.Question;
using PropayTest.Models.Quiz;
using PropayTest.Models.QuizzesUsers;
using PropayTest.Models.SolvedQuiz;
using System.Data.SqlClient;

namespace PropayTest.Services
{
    public class QuizService
    {
        public static List<PopularQuiz> GetPopularQuizzes()
        {
            List<PopularQuiz> popularQuizzes = new List<PopularQuiz>();
            try
            {
                String connectionString = "Data Source=LEGION\\SQLEXPRESS;Initial Catalog=PropayTest;Integrated Security=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "select\r\n\tUserQuizResults.QuizId,\r\n\tUsers.UserName as Creator,\r\n\tQuizzes.Title,\r\n\tQuizzes.Description,\r\n\tcount(*) as occurence\r\nfrom Quizzes\r\ninner join UserQuizResults on Quizzes.Id = UserQuizResults.QuizId\r\ninner join Users on Quizzes.CreatorId = Users.Id\r\ngroup by UserQuizResults.QuizId, Users.UserName, Quizzes.Title, Quizzes.Description\r\norder by occurence desc";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        
                        // Execute the command
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                PopularQuiz quiz = new PopularQuiz();
                                quiz.QuizId = reader.GetInt32(0);
                                quiz.CreatorUserName = reader.GetString(1);
                                quiz.Title = reader.GetString(2);
                                quiz.Description = reader.GetString(3);
                                quiz.Occurence = reader.GetInt32(4);
                                

                                popularQuizzes.Add(quiz);
                            }
                        }
                    }
                }
                return popularQuizzes;
            }

            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return popularQuizzes;
        }



        public static List<SolvedQuiz> GetAllSolvedQuizzes(int UserId)
        {
            List<SolvedQuiz> solvedQuizzes = new List<SolvedQuiz>();
            try
            {
                String connectionString = "Data Source=LEGION\\SQLEXPRESS;Initial Catalog=PropayTest;Integrated Security=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "select \r\nUserQuizResults.ResultId, \r\nUserQuizResults.QuizId, \r\nUserQuizResults.UserId,\r\nUserQuizResults.Score,\r\nUserQuizResults.CompletedDate,\r\nUserQuizResults.HowManyCorrect,\r\nUserQuizResults.HowManyWrong,\r\n\r\nQuizzes.CreatorId,\r\nQuizzes.Title,\r\nQuizzes.Description,\r\n\r\nUsers.UserName\r\n\r\nfrom UserQuizResults inner join Quizzes\r\non UserQuizResults.QuizId = Quizzes.Id\r\n\r\ninner join Users on Users.Id = Quizzes.CreatorId\r\n\r\nwhere UserId = @userId order by CompletedDate desc";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        // Add parameters to the command
                        command.Parameters.AddWithValue("@userId", UserId);

                        // Execute the command
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                SolvedQuiz quiz = new SolvedQuiz();
                                quiz.ResultId = reader.GetInt32(0);
                                quiz.QuizId = reader.GetInt32(1);
                                quiz.UserId = reader.GetInt32(2);
                                quiz.Score = reader.GetString(3);
                                quiz.CompletedDate = reader.GetDateTime(4);
                                quiz.HowManyCorrect = reader.GetInt32(5);
                                quiz.HowManyWrong = reader.GetInt32(6);
                                quiz.CreatorId = reader.GetInt32(7);
                                quiz.Title = reader.GetString(8);
                                quiz.Description = reader.GetString(9);
                                quiz.CreatorUserName = reader.GetString(10);

                                DateTime pastDateTime = quiz.CompletedDate;
                                DateTime currentDateTime = DateTime.Now;
                                TimeSpan timeDifference = currentDateTime - pastDateTime;

                                string timeAgoMessage = GetTimeAgoMessage(timeDifference);

                                quiz.Solved = timeAgoMessage;

                                solvedQuizzes.Add(quiz);
                            }
                        }
                    }
                }
                return solvedQuizzes;
            }

            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return solvedQuizzes;
        }


        public static List<Quiz> GetAllQuizzes(int UserId)
        {
            List<Quiz> quizzes = new List<Quiz>();
            try
            {
                String connectionString = "Data Source=LEGION\\SQLEXPRESS;Initial Catalog=PropayTest;Integrated Security=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "SELECT * FROM Quizzes where CreatorId=@creatorId order by CreatedDate desc";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        // Add parameters to the command
                        command.Parameters.AddWithValue("@creatorId", UserId);

                        // Execute the command
                        using (SqlDataReader reader = command.ExecuteReader())
                        {

                            while (reader.Read())
                            {
                                Quiz quiz = new Quiz();
                                quiz.Id = reader.GetInt32(0);
                                quiz.Title = reader.GetString(1);
                                quiz.Description = reader.GetString(2);
                                quiz.CreatedDate = reader.GetDateTime(3);
                                quiz.IsActive = reader.GetBoolean(4);
                                quiz.CreatorId = reader.GetInt32(5);
                                
                                quizzes.Add(quiz);
                            }
                        }
                    }
                }

            }

            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return quizzes;
        }


        public static List<QuizUser> GetEveryQuiz()
        {
            List<QuizUser> quizUser = new List<QuizUser>();
            try
            {
                String connectionString = "Data Source=LEGION\\SQLEXPRESS;Initial Catalog=PropayTest;Integrated Security=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "use PropayTest\r\nselect Quizzes.Id as QuizId, Quizzes.Title, Quizzes.Description, Quizzes.CreatedDate, CreatorId, Users.FullName, Users.Id as UserId from Quizzes\r\ninner join Users\r\non Quizzes.CreatorId = Users.Id";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        // Add parameters to the command
                        //command.Parameters.AddWithValue("@creatorId", UserId);

                        // Execute the command
                        using (SqlDataReader reader = command.ExecuteReader())
                        {

                            while (reader.Read())
                            {
                                QuizUser quiz = new QuizUser();
                                quiz.QuizId = reader.GetInt32(0);
                                quiz.Title= reader.GetString(1);
                                quiz.Description = reader.GetString(2);
                                quiz.CreatedDate = reader.GetDateTime(3);
                                quiz.CreatorId= reader.GetInt32(4);
                                quiz.FullName = reader.GetString(5);
                                quiz.UserId = reader.GetInt32(6);

                                quizUser.Add(quiz);
                            }
                        }
                    }
                }

            }

            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return quizUser;
        }

        static string GetTimeAgoMessage(TimeSpan timeDifference)
        {
            if (timeDifference.TotalMinutes < 1)
            {
                return "just now";
            }
            else if (timeDifference.TotalMinutes < 60)
            {
                return $"{(int)timeDifference.TotalMinutes} minutes ago";
            }
            else if (timeDifference.TotalHours < 24)
            {
                return $"{(int)timeDifference.TotalHours} hours ago";
            }
            else
            {
                return $"{(int)timeDifference.TotalDays} days ago";
            }
        }

    }
}
