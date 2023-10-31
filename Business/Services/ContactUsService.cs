
using Business.Interfaces;
using Common.Utilities;
using Communication.Interfaces;
using Domain.Interfaces;
using Domain.Models;
using Microsoft.Extensions.Configuration;
using Models.Requests.ContactUs;
using Models.Responses;
using Models.ServiceRequest;

namespace Business.Services
{
    public class ContactUsService : IContactUsService
    {
        private readonly IContactUsRepository _contactUsRepository;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;

        public ContactUsService(IContactUsRepository contactUsRepository, IConfiguration configuration, IEmailService emailService)
        {
            _contactUsRepository = contactUsRepository;
            _configuration = configuration;
            _emailService = emailService;
        }
        public async Task<ApiResponse<bool>> AddContactUs(ContactRequest request)
        {
            if (request.IsDataStored.HasValue && request.IsDataStored.Value == true)
            {
                var data = new ContactUs
                {
                    CreatedDate = request.CreatedDate,
                    EmailAddress = request.EmailAddress,
                    Name = request.Name,
                    Message = request.Message,
                    IsDataStored = request.IsDataStored
                };
                var flag = await _contactUsRepository.AddContactUs(data);
                if (!flag)
                {
                    return new ApiResponse<bool> { Result = false, Success = false , ErrorMessage = "unable to insert customer data!"};
                }
                // email sending
                var flagEmail = await SendContactUsEmail(request);
                if (!flagEmail)
                {
                    return new ApiResponse<bool> { Success = true, Result = flag, ErrorMessage = "There is an error while sending email" };
                }
                return new ApiResponse<bool> { Result = flag, Success = true, Message = "contact us record entered successfully!" };
            }
            return new ApiResponse<bool> { Result = false, Success = false, ErrorMessage = "please provide the customer data!" };
        }

        public async Task<ApiResponse<List<ContactUs>>> GetAllContactUs()
        {
            var result = await _contactUsRepository.GetAllContactUs();
            if (result != null && result.Count > 0)
            {
                return new ApiResponse<List<ContactUs>> { Result = result, Success = true };
            }
            return new ApiResponse<List<ContactUs>> { Result = null, Success = true, Message = "No record found " };
        }

        private async Task<bool> SendContactUsEmail(ContactRequest request)
        {
            var mailRequest = new MailRequest
            {
                ToEmail = _configuration.GetSection("EmailConfig").GetSection("ContactEmail").Value,
                Body = Utility.ContactUsEmailBody(request),
                Subject = "Sample App - Contact Us",
                Attachments = null
            };

            var emailFlag = await _emailService.SendEmailAsync(mailRequest);
            if (emailFlag)
            {
                return true;
            }
            return false;
        }

    }
}
