
using Data.Context;
using Domain.Interfaces;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Models.Requests.ContactUs;

namespace Data.Repositories
{
    public class ContactUsRepository : IContactUsRepository
    {
        private readonly ApplicationDbContext _db;

        public ContactUsRepository(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<bool> AddContactUs(ContactUs request)
        {
            var result = await _db.ContactUs.AddAsync(request);
            if(result != null)
            {
                _db.SaveChanges();
                return true;
            }
            return false;
        }

        public async Task<List<ContactUs>> GetAllContactUs()
        {
            return await _db.ContactUs.ToListAsync();
        }
    }
}
