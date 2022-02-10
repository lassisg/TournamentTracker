using System;

using MailKit.Net.Smtp;
using MailKit;
using MimeKit;

namespace TrackerLibrary
{
    public class EmailLogic
    {
        public static void SendEmail(string to, string subject, string body)
        {
            MailboxAddress fromMailAddress = new MailboxAddress(
                GlobalConfig.AppKeyLookup("senderDisplayName"),
                GlobalConfig.AppKeyLookup("senderEmail"));

            MailboxAddress toMailAddress = new MailboxAddress("", to);

            MimeMessage mail = new MimeMessage();
            mail.To.Add(toMailAddress);
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