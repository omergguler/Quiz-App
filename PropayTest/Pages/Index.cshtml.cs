using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PropayTest.Models.Email;
using PropayTest.Pages.Users;
using PropayTest.Services;
using System.Data.SqlClient;
using System.Net.Mail;
using System.Net;
namespace PropayTest.Pages
{
    public class IndexModel : PageModel
    {

        User user = new User();
        List<User> usersList = new List<User>();

        public string successMessage = "";
        public string errorMessage = "";
        public string sixDigitNumber = "";
        public string userNameOrEmail = "";

        public bool wrongDigits = false;
        public bool resend = false;
        public bool confirmed = true;
        public string firstName = "";
        public string userEmail = "";
        

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostResend(string email)
        {
            Random random = new Random();
            sixDigitNumber = "";
            userEmail = email;
            for (int i = 0; i < 6; i++)
            {
                int randomDigit = random.Next(0, 10);
                sixDigitNumber += randomDigit.ToString();
            }

            try
            {
                String connectionString = "Data Source=LEGION\\SQLEXPRESS;Initial Catalog=PropayTest;Integrated Security=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "Update Users set SixDigitNumber = @randomNumber where Email = @email";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        // Add parameters to the command
                        command.Parameters.AddWithValue("@randomNumber", sixDigitNumber);
                        command.Parameters.AddWithValue("@email", email);

                        command.ExecuteNonQuery();
                    }
                }
            }

            catch (Exception e)
            {
                errorMessage = e.Message;
            }


            await SendEmail(email, sixDigitNumber);

            wrongDigits = false;
            confirmed = false;
            resend = true;
            return Page();
        }

        public IActionResult OnPostVerify(string first, string second, string third, string fourth, string fifth, string sixth, string email)
        {
            sixDigitNumber = "";
            sixDigitNumber = "" + first + second + third + fourth + fifth + sixth;
            usersList.Clear();
            userEmail = email;

            try
            {
                String connectionString = "Data Source=LEGION\\SQLEXPRESS;Initial Catalog=PropayTest;Integrated Security=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "Select * from Users where SixDigitNumber = @sixDigitNumber and Email = @email";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        // Add parameters to the command
                        command.Parameters.AddWithValue("@sixDigitNumber", sixDigitNumber);
                        command.Parameters.AddWithValue("@email", email);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                user.Id = reader.GetInt32(0);
                                user.UserName = reader.GetString(1);
                                user.FullName = reader.GetString(2);
                                user.Email = reader.GetString(3);
                                user.PasswordHash = reader.GetString(4);
                                user.SecurityStamp = reader.GetString(5);
                                user.EmailConfirmed = reader.GetBoolean(6);
                                user.CreatedDate = reader.GetDateTime(7);
                                user.SixDigitNumber = reader.GetString(8);
                                usersList.Add(user);
                            }
                        }
                    }
                }

                if (usersList.Count == 1) // means there is a match and unique
                {
                    // set user to confirmed and direct to profile

                    try
                    {
                        using (SqlConnection connection = new SqlConnection(connectionString))
                        {
                            connection.Open();
                            string sql = "Update Users set EmailConfirmed = @emailConfirmed where SixDigitNumber = @sixDigitNumber and Email = @email";

                            using (SqlCommand command = new SqlCommand(sql, connection))
                            {
                                // Add parameters to the command
                                command.Parameters.AddWithValue("@sixDigitNumber", sixDigitNumber);
                                command.Parameters.AddWithValue("@email", email);
                                command.Parameters.AddWithValue("@emailConfirmed", 1);

                                command.ExecuteNonQuery();
                            }
                        }
                        HttpContext.Session.SetInt32("UserId", usersList[0].Id);
                        confirmed = true;
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
                    confirmed = false;
                    errorMessage = "match count: " + usersList.Count;
                    wrongDigits = true;
                    return Page();
                }

            }

            catch (Exception e)
            {
                errorMessage = e.Message;
                return Page();
            }

        }

        public async Task<IActionResult> OnPost()
        {

            userNameOrEmail = Request.Form["login-username-email"]!;
            string password = Request.Form["login-password"]!;
            usersList.Clear();

            user = UserCredentialsValidator.validAndConfirmed(userNameOrEmail);

            if (user != null)
            {
                
                // user
                if (user.PasswordHash == PasswordHasher.HashPassword(password))
                {
                    // user, password
                    if (user.EmailConfirmed)
                    {
                        // great!
                        successMessage = "Signed in!";
                        HttpContext.Session.SetInt32("UserId", user.Id);
                        confirmed = true;
                        return Redirect("/profile");

                    }
                    else
                    {
                        // user, password, not confirmed
                        errorMessage = "Verify your account first.";
                        sixDigitNumber = "";
                        usersList.Clear();

                        firstName = user.FullName.Substring(0, user.FullName.IndexOf(' '));
                        userEmail = user.Email;

                        Random random = new Random();
                        sixDigitNumber = "";

                        for (int i = 0; i < 6; i++)
                        {
                            int randomDigit = random.Next(0, 10);
                            sixDigitNumber += randomDigit.ToString();
                        }

                        try
                        {
                            String connectionString = "Data Source=LEGION\\SQLEXPRESS;Initial Catalog=PropayTest;Integrated Security=True";
                            using (SqlConnection connection = new SqlConnection(connectionString))
                            {
                                connection.Open();
                                string sql = "Update Users set SixDigitNumber = @randomNumber where UserName = @userName";

                                using (SqlCommand command = new SqlCommand(sql, connection))
                                {
                                    // Add parameters to the command
                                    command.Parameters.AddWithValue("@randomNumber", sixDigitNumber);
                                    command.Parameters.AddWithValue("@userName", user.UserName);

                                    command.ExecuteNonQuery();
                                }
                            }
                        }

                        catch (Exception e)
                        {
                            errorMessage = e.Message;
                        }
                        confirmed = false;
                        
                        await SendEmail(user.Email, sixDigitNumber);
                        return Page();

                    }

                }

                // user, wrong password
                else
                {
                    errorMessage = "Wrong password.";
                    return Page();
                }
            }
            else
            {
                // no user
                errorMessage = "No account found.";
                return Page();
            }


        }

        private async Task SendEmail(string toEmail, string sixDigit)
        {
            try
            {
                // Configure the email
                var smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587, // Adjust if needed
                    Credentials = new NetworkCredential("enter mail", "enter password"),
                    EnableSsl = true,
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress("enter mail"),
                    Subject = "Email Verification Code",
                    Body = "Dear user, enter the code to verify your email: " + sixDigit,
                    IsBodyHtml = false, // Set to true if you want to send an HTML email
                };

                mailMessage.To.Add(toEmail);

                // Send the email
                await smtpClient.SendMailAsync(mailMessage);
            }
            catch (Exception ex)
            {
                errorMessage = "Error sending email: " + ex.Message;
            }
        }
    }

}
