using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PropayTest.Models.Quiz;
using PropayTest.Pages.Users;
using PropayTest.Services;

namespace PropayTest.Pages.Bio
{
    public class BioModel : PageModel
    {
        public User? User { get; set; }

        public List<Quiz> Quizzes { get; set; }

        public int questionCount { get; set; }

        public int quizCount { get; set; }

        public int rank { get; set; }
        public int weight { get; set; }

        public void OnGet()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId != null)
            {
                rank = 0;
                User = UserService.GetUserDetails((int)userId);
                Quizzes = QuizService.GetAllQuizzes((int)userId);
                questionCount = (int)HttpContext.Session.GetInt32("QuestionCount");
                quizCount = Quizzes.Count;
                rank = (int)HttpContext.Session.GetInt32("Rank");
                try
                {
                    weight = (int)HttpContext.Session.GetInt32("Weight");
                }
                catch (Exception e)
                {

                }
                
            }
        }
        public IActionResult OnPostSignOut()
        {
            HttpContext.Session.Remove("UserId");
            User = null;
            return RedirectToPage("/Index");
        }
    }
}
