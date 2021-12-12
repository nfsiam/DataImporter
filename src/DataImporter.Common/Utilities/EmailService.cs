using System.IO;
using System.Threading.Tasks;
using DataImporter.Common.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace DataImporter.Common.Utilities
{
    public class EmailService : IEmailService
    {
        private readonly SmtpConfiguration _smtpConfiguration;

        public EmailService(SmtpConfiguration smtpConfiguration)
        {
            _smtpConfiguration = smtpConfiguration;
        }

        public async Task SendEmailAsync(string receiver, string subject, string body)
        {
            await SendEmailAsync(receiver, subject, body, null);
        }

        public async Task SendEmailAsync(string receiver, string subject, string body, MemoryStream stream,
            string fileName = "")
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_smtpConfiguration.FromName ?? _smtpConfiguration.FromEmail,
                _smtpConfiguration.FromEmail));
            message.To.Add(new MailboxAddress(receiver, receiver));
            message.Subject = subject;

            var bodyBuilder = new BodyBuilder();

            bodyBuilder.HtmlBody = body;

            if (stream != null && !string.IsNullOrEmpty(fileName))
            {
                stream.Position = 0;
                await bodyBuilder.Attachments.AddAsync(fileName, stream);
            }

            message.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();
            client.Timeout = 60000;
            client.ServerCertificateValidationCallback = (s, c, h, e) => true;
            client.Connect(_smtpConfiguration.Host, _smtpConfiguration.Port,
                _smtpConfiguration.UseSsl ? SecureSocketOptions.SslOnConnect : SecureSocketOptions.StartTls);
            client.Authenticate(_smtpConfiguration.Username, _smtpConfiguration.Password);
            client.Send(message);
            client.Disconnect(true);
        }
    }
}