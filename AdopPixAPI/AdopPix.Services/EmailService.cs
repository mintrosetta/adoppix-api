using AdopPixAPI.Services.IServices;
using Microsoft.Extensions.Configuration;
using System.Net.Mail;
using System.Net;

namespace AdopPixAPI.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration configuration;

        public EmailService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        public string CreateTemplate(string emailType, string header, string subHeader, string url)
        {
            string html = "";
            using(WebClient client = new WebClient())
            {
                html = client.DownloadString($"https://cdn.adoppix.com/mailtemplate/{emailType}.html");
            }

            html = html.Replace("[header]", header);
            html = html.Replace("[subHeader]", subHeader);
            html = html.Replace("[urlConfirmEmail]", url);
            return html;
        }

        public async Task SendAsync(string to, string subject, string body)
        {
            var SMTP_Host = configuration.GetSection("Email:SMTP_Host").Value;
            var SMTP_Port = configuration.GetSection("Email:SMTP_Port").Value;
            var SMTP_User = configuration.GetSection("Email:SMTP_User").Value;
            var SMTP_Password = configuration.GetSection("Email:SMTP_Password").Value;
            var SENDER = configuration.GetSection("Email:SMTP_Sender").Value;

            MailMessage message = new MailMessage(SENDER, to, subject, body);
            message.IsBodyHtml = true;

            using (var smtpClient = new SmtpClient(SMTP_Host, Convert.ToInt16(SMTP_Port)))
            {
                smtpClient.Credentials = new NetworkCredential(SMTP_User, SMTP_Password);
                await smtpClient.SendMailAsync(message);
            }
        }
    }
}
