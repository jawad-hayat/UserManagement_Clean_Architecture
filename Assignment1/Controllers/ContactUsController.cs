using Business.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Models.Requests.ContactUs;

namespace Assignment1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactUsController : ControllerBase
    {
        private readonly IContactUsService _contactUsService;

        public ContactUsController(IContactUsService contactUsService)
        {
            _contactUsService = contactUsService;
        }

        [HttpPost("addcontactus")]
        public async Task<IActionResult> AddContactUs([FromBody] ContactRequest request)
        {
            return new OkObjectResult(await _contactUsService.AddContactUs(request));
        }

        [HttpGet("getall")]
        public async Task<IActionResult> GetAllContactUs()
        {
            return new OkObjectResult(await _contactUsService.GetAllContactUs());
        }
    }
}
