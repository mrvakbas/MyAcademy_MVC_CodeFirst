using MimeKit;
using MailKit.Net.Smtp;

namespace MyAcademy_MVC_CodeFirst.Services
{
    public class EmailService
    {
        public void SendAutoReply(string receiverEmail, string receiverName, string subject, string messageBody)
        {
            var mimeMessage = new MimeMessage();
            mimeMessage.From.Add(new MailboxAddress("LifeSure Sigorta", "merve.akba54@gmail.com"));
            mimeMessage.To.Add(new MailboxAddress(receiverName, receiverEmail));
            mimeMessage.Subject = "Mesajınız Alındı: " + subject;
            var bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = $@"
            <div style='font-family: Arial, sans-serif; border: 1px solid #eee; padding: 20px;'>
                <h2 style='color: #4e73df;'>LifeSure Sigorta</h2>
                <p>{messageBody}</p>
                <hr>
                <small style='color: #888;'>Bu bir otomatik yanıttır, lütfen bu maili cevaplamayınız.</small>
            </div>";
            mimeMessage.Body = bodyBuilder.ToMessageBody();
             using (var client = new SmtpClient())
            {
                client.Connect("smtp.gmail.com", 587, false);
                client.Authenticate("merve.akba54@gmail.com", "sbgkqcbyyuuyubjt");
                client.Send(mimeMessage);
                client.Disconnect(true);
            }
        }
    }
}