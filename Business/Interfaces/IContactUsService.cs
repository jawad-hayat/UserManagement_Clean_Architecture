
using Domain.Models;
using Models.Requests.ContactUs;
using Models.Responses;

namespace Business.Interfaces
{
    public interface IContactUsService
    {
        Task<ApiResponse<List<ContactUs>>> GetAllContactUs();
        Task<ApiResponse<bool>> AddContactUs(ContactRequest request);
    }
}
