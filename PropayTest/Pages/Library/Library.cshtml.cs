using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PropayTest.Models.Question;
using PropayTest.Models.Quiz;
using PropayTest.Pages.Users;
using PropayTest.Services;

namespace PropayTest.Pages.Library
{
    public class LibraryModel : PageModel
    {
        public User? User { get; set; }
        public List<Quiz> Quizzes { get; set; }

        public List<Question> Questions { get; set; }


        public void OnGet()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId != null)
            {
                User = UserService.GetUserDetails((int)userId);
                Quizzes = QuizService.GetAllQuizzes((int)userId);
                Questions = QuestionService.GetAllQuestions((int)userId);
            }
        }
    }
}
