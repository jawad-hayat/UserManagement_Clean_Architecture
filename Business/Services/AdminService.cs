using Business.Interfaces;
using Microsoft.AspNetCore.Identity;
using Models.Requests.Admin;
using Models.Responses;
using Models.Responses.Admin;

namespace Business.Services
{
    public class AdminService : IAdminService
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminService(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }
        public async Task<ApiResponse<AdminResponse>> CreateRole(RoleRequest request)
        {
            IdentityRole identityRole = new IdentityRole
            {
                Name = request.Name
            };
            IdentityResult result = await _roleManager.CreateAsync(identityRole);
            if (result.Succeeded)
            {
                return new ApiResponse<AdminResponse>()
                {
                    Message = "Role Created Successfully",
                    Success = true
                };
            }
            return new ApiResponse<AdminResponse>()
            {
                ErrorMessage = "Role Already Exists",
                Success = false
            };
        }
    }
}
