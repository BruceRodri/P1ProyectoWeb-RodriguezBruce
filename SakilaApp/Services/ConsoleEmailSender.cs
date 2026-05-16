using Microsoft.AspNetCore.Identity.UI.Services; // IMPORTANTE: USAR ESTE NAMESPACE
using Microsoft.Extensions.Logging;

namespace SakilaApp.Services
{
    public class ConsoleEmailSender : IEmailSender
    {
        private readonly ILogger<ConsoleEmailSender> _logger;

        public ConsoleEmailSender(ILogger<ConsoleEmailSender> logger)
        {
            _logger = logger;
        }

        // EL NOMBRE DEL ÚLTIMO PARÁMETRO DEBE SER htmlMessage
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            _logger.LogInformation($@"ENVIANDO CORREO A: {email}
            ASUNTO: {subject}
            CUERPO: {htmlMessage}");

            return Task.CompletedTask;
        }
    }
}