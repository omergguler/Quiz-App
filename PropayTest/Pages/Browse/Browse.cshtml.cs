using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PropayTest.Models.Question;
using PropayTest.Models.Quiz;
using PropayTest.Models.QuizzesUsers;
using PropayTest.Pages.Users;
using PropayTest.Services;
using System.Data.SqlClient;

namespace PropayTest.Pages.Browse
{
    public class BrowseModel : PageModel
    {
        public User? User { get; set; }
        public List<QuizUser> QuizzesUsers { get; set; }

        public List<Question> Questions { get; set; }

        public string errorMessage = "";

        public void OnGet()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId != null)
            {
                // Fetch user details from the database using userId
                User = UserService.GetUserDetails((int)userId);
                QuizzesUsers = QuizService.GetEveryQuiz();
            }
        }

        public IActionResult OnPostTakeQuiz(string SelectedQuizId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId != null)
            {
                string url = "/take-quiz?id=" + SelectedQuizId;
                return Redirect(url);

            }

            else
            {
                return RedirectToPage("/Index");
            }


        }

        
    }
}
