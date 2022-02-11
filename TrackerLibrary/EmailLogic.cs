using MailKit.Net.Smtp;
using MimeKit;

namespace TrackerLibrary
{
    public class EmailLogic
    {
        public static void SendEmail(string to, string subject, string body)
        {
            SendEmail(new List<string> { to }, new List<string>(), subject, body);
        }

        public static void SendEmail(List<string> to, List<string> bcc, string subject, string body)
        {
            MailboxAddress fromMailAddress = new MailboxAddress(
                GlobalConfig.AppKeyLookup("senderDisplayName"),
                GlobalConfig.AppKeyLookup("senderEmail"));

            MimeMessage mail = new MimeMessage();

            foreach (string email in to)
            {
                mail.To.Add(MailboxAddress.Parse(email));
            }

            foreach (string email in bcc)
            {
                mail.Bcc.Add(MailboxAddress.Parse(email));
            }

            mail.From.Add(fromMailAddress);
            mail.Subject = subject;

            var bodyBuilder = new BodyBuilder();

            bodyBuilder.HtmlBody = body;
            bodyBuilder.TextBody = "-";

            mail.Body = bodyBuilder.ToMessageBody();

            var client = new SmtpClient();

            client.Connect(
                host: GlobalConfig.AppKeyLookup("smtpHost"),
                port: int.Parse(GlobalConfig.AppKeyLookup("smtpPort")),
                useSsl: bool.Parse(GlobalConfig.AppKeyLookup("smtpUseSsl")));

            client.Send(mail);
        }
    }
}