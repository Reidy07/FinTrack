using FinTrack.Core.Interfaces.Services;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;

namespace FinTrack.Infrastructure.Services
{
    public class EmailSender : IEmailSender, IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly IEmailTemplateService _templateService;

        public EmailSender(IConfiguration configuration, IEmailTemplateService templateService)
        {
            _configuration = configuration;
            _templateService = templateService;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var host = _configuration["EmailSettings:Host"];
            var port = int.Parse(_configuration["EmailSettings:Port"] ?? "587");
            var user = _configuration["EmailSettings:Username"];
            var pw = _configuration["EmailSettings:Password"];
            var senderEmail = _configuration["EmailSettings:SenderEmail"];
            var senderName = _configuration["EmailSettings:SenderName"];

            if (string.IsNullOrEmpty(host)) return;

            var client = new SmtpClient(host, port)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(user, pw)
            };

            var finalHtml = await _templateService.GetMasterTemplateAsync(subject, htmlMessage);

            var mailMessage = new MailMessage
            {
                From = new MailAddress(senderEmail!, senderName),
                Subject = subject,
                Body = finalHtml,
                IsBodyHtml = true
            };
            mailMessage.To.Add(email);

            await client.SendMailAsync(mailMessage);
        }
    }
}