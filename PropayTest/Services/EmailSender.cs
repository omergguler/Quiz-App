
using System.Net;
using System.Net.Mail;

namespace PropayTest.Services
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string message)
        {
            var mail = "add your mail adress";
            var pw = "add password";

            var client = new SmtpClient("smtp-mail.outlook.com", 587) // depends on the mail client that is used, check
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(mail, pw)
            };

            return client.SendMailAsync(
                new MailMessage(from: mail,
                to: email, subject, message));
        }
    }
}
