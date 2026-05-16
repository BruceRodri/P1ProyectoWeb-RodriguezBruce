/*using Microsoft.Extensions.Logging;

namespace SakilaApp.Services
{
    public class ConsoleEmailSender : IEmailSender
    {
        private readonly ILogger<ConsoleEmailSender> _logger;

        public ConsoleEmailSender(ILogger<ConsoleEmailSender> logger)
        {
            _logger = logger;
        }

        public Task SendEmailAsync(string to, string subject, string body)
        {
            // USAMOS $@ PARA PERMITIR SALTOS DE LÍNEA DENTRO DE LAS COMILLAS
            _logger.LogInformation($@"Enviando email a {to}:
            Asunto: {subject}
            Cuerpo: {body}");

            return Task.CompletedTask;
        }
    }
}*/