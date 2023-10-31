
using Models.ServiceRequest;

namespace Communication.Interfaces
{
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(MailRequest mailRequest);
    }
}
