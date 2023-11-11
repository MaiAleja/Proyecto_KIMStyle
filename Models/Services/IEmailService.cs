using KIM_Style.Models.DTO;

namespace KIM_Style.Models.Services
{
    public interface IEmailService
    {
        void SendEmail(EmailDTO request);
    }
}
