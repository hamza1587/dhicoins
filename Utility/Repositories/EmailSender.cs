using System.Net.Mail;
using System.Net;

namespace Dhicoin.Utility.Repositories
{

    public class EmailSender : IMailSender
    {
        public void SendEmail(Message message)
        {
            SendEmail(message.Subject, message.Content, message.To);
        }
        public void SendEmail(string subject, string body, string toAddress)
        {
            try
            {
                var mail = new MailMessage();
                var client = new System.Net.Mail.SmtpClient("smtp.gmail.com", 587) //Port 8025,2525 and 25 can also be used.
                {
                    Credentials = new NetworkCredential("maryamghafoor20@gmail.com", "jtdamjffmnrayccg"),
                    EnableSsl = true
                };
                mail.From = new MailAddress("maryamghafoor20@gmail.com", "Maryam");
                mail.To.Add(toAddress);
                mail.Subject = subject;
                var htmlView = AlternateView.CreateAlternateViewFromString(body, null, "text/html");
                mail.AlternateViews.Add(htmlView);
                client.Send(mail);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}

