using MailKit.Security;
using MimeKit.Text;
using MimeKit;
using MailKit.Net.Smtp;
using KIM_Style.Models.DTO;

namespace KIM_Style.Models.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            this._config = config;
        }

        public void SendEmail(EmailDTO request)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_config.GetSection("Email:UserName").Value));
            email.To.Add(MailboxAddress.Parse(request.Para));
            email.Subject = request.Asunto;
            email.Body = new TextPart(TextFormat.Html)
            { 
                Text = request.contenido
            };

            using var smtp = new SmtpClient();
            smtp.Connect(
                _config.GetSection("Email:Host").Value,
                Convert.ToInt32(_config.GetSection("Email:Port").Value),
                SecureSocketOptions.StartTls
                );

            smtp.Authenticate(_config.GetSection("Email:UserName").Value, _config.GetSection("Email:PassWord").Value);
            smtp.Send(email);
            smtp.Disconnect(true);
        }

    }
}
