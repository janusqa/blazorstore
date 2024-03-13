
using MailKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using MimeKit;

namespace Bookstore.Utility
{
    // we have mocked this class until we get to its implementation as it
    // will cause an error when seeding roles to the DB

    public class SmtpEmailSender : IEmailSender
    {
        private readonly string _server;
        private readonly int _port;
        private readonly string _uid;
        private readonly string _pid;



        public SmtpEmailSender(IConfiguration _config)
        {
            _server = _config.GetValue<string>("SmtpServer:Server") ?? "";
            _port = _config.GetValue<int>("SmtpServer:Port");
            _uid = _config.GetValue<string>("Smtp:uid") ?? "";
            _pid = _config.GetValue<string>("Smtp:pid") ?? "";
        }

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            var emailMessage = new MimeMessage
            {
                Sender = MailboxAddress.Parse(_uid)
            };
            emailMessage.To.Add(MailboxAddress.Parse(email));
            emailMessage.Subject = subject;
            var body = new BodyBuilder
            {
                HtmlBody = message
            };
            emailMessage.Body = body.ToMessageBody();

            using var smtp = new SmtpClient(new ProtocolLogger(Console.OpenStandardOutput()));
            smtp.Connect(_server, _port, SecureSocketOptions.StartTls);
            smtp.Authenticate(_uid, _pid);
            var result = await smtp.SendAsync(emailMessage);
            smtp.Disconnect(true);
            return;
            // return Task.CompletedTask; // use this to mock a service.
        }
    }
}