
using Domain.Models;
using Models.Requests.ContactUs;

namespace Domain.Interfaces
{
    public interface IContactUsRepository
    {
        Task<List<ContactUs>> GetAllContactUs();
        Task<bool> AddContactUs(ContactUs request);
    }
}
