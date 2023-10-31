using Business.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Requests.Admin;

namespace Assignment1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }
        [HttpPost("create_role")]
        
        public async Task<IActionResult> CreateRole(RoleRequest role)
        {
            if (ModelState.IsValid)
            {
                var result =  await _adminService.CreateRole(role);
                return Ok(result);
            }
            return BadRequest("provide name");
        }

    }
}
