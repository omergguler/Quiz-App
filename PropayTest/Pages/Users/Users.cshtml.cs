using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using PropayTest.Services;

namespace PropayTest.Pages.Users
{
    public class UsersModel : PageModel
    {
        public List<User> usersList = new List<User>();
        public void OnGet()
        {
            try
            {
                String connectionString = "Data Source=LEGION\\SQLEXPRESS;Initial Catalog=PropayTest;Integrated Security=True;Trust Server Certificate=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    String sql = "Select * from Users";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {

                            while (reader.Read())
                            {
                                User userInfo = new User();
                                userInfo.Id = reader.GetInt32(0);
                                userInfo.UserName = reader.GetString(1);
                                userInfo.FullName = reader.GetString(2);
                                userInfo.Email = reader.GetString(3);
                                userInfo.PasswordHash = reader.GetString(4);
                                userInfo.SecurityStamp = reader.GetString(5);
                                userInfo.EmailConfirmed = reader.GetBoolean(6);
                                userInfo.CreatedDate = reader.GetDateTime(7);
                                usersList.Add(userInfo);
                            }
                        }
                    }
                }

            }
            catch { }
        }
    }

    public class User
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string SecurityStamp { get; set; }
        public bool EmailConfirmed { get; set; }  
        public DateTime CreatedDate { get; set; } 
        public string SixDigitNumber { get; set; }

    }
}
