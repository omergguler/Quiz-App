using Microsoft.AspNetCore.Mvc.RazorPages;
using PropayTest.Models.Email;
using PropayTest.Pages.Users;
using PropayTest.Services;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Net.Mail;
using System.Net;
using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;


namespace PropayTest.Pages.Register
{
    public class RegisterModel : PageModel
    {
        public User user = new User();
        List<User> usersList = new List<User>();

        public string successMessage = "";
        public string errorMessage = "";
        public string sixDigitNumber = "";
        public string userNameOrEmail = "";

        public bool wrongDigits = false;
        public bool resend = false;
        public bool confirmed = true;
        public string firstName = "";

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostResend(string email)
        {
            Random random = new Random();
            sixDigitNumber = "";
            user.Email = email;
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
            user.Email = email;
            usersList.Clear();
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
                    errorMessage = "match count: " + usersList.Count();
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

        public async Task OnPost()
        {
            user.UserName = Request.Form["register-username"].ToString();
            user.Email = Request.Form["register-email"].ToString();
            user.FullName = Request.Form["register-name"].ToString() + " " + Request.Form["register-surname"].ToString();
            user.PasswordHash = PasswordHasher.HashPassword(Request.Form["register-newPassword"].ToString());

            if (UserUniquenessChecker.isUniqueUser(user.UserName, user.Email))
            {

                Random random = new Random();
                string randomNumber = "";

                for (int i = 0; i < 6; i++)
                {
                    int randomDigit = random.Next(0, 10);
                    randomNumber += randomDigit.ToString();
                }                

                // save user to database
                try
                {
                    String connectionString = "Data Source=LEGION\\SQLEXPRESS;Initial Catalog=PropayTest;Integrated Security=True";
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        string sql = "INSERT INTO Users " +
                     "(UserName, FullName, Email, PasswordHash, SecurityStamp, EmailConfirmed, CreatedDate, SixDigitNumber) VALUES " +
                     "(@userName, @fullName, @email, @passwordHash, @securityStamp, @emailConfirmed, @createdDate, @sixDigitNumber)";

                        using (SqlCommand command = new SqlCommand(sql, connection))
                        {
                            // Add parameters to the command
                            command.Parameters.AddWithValue("@userName", user.UserName);
                            command.Parameters.AddWithValue("@fullName", user.FullName);
                            command.Parameters.AddWithValue("@email", user.Email);
                            command.Parameters.AddWithValue("@passwordHash", user.PasswordHash);
                            command.Parameters.AddWithValue("@securityStamp", "stamped");
                            command.Parameters.AddWithValue("@emailConfirmed", 0);
                            command.Parameters.AddWithValue("@createdDate", DateTime.Now);
                            command.Parameters.AddWithValue("@sixDigitNumber", randomNumber);
                            // Execute the command
                            command.ExecuteNonQuery();
                        }
                    }

                    firstName = Request.Form["register-name"].ToString();
                    // Send email to the user
                    await SendEmail(user.Email, randomNumber);
                }

                catch (Exception e)
                {
                    errorMessage = e.Message;
                    return;
                }

                confirmed = false;
                successMessage = "Created user successfully!";
            }
            else
            {
                errorMessage = "pick a unique username/email";
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

