using Models.Responses.Accounts;
using Models.Responses;
using static Models.Requests.Accounts.AccountRequest;
using Models.Responses.Admin;
using Models.Requests.Admin;

namespace Business.Interfaces
{
    public interface IAdminService
    {
        Task<ApiResponse<AdminResponse>> CreateRole(RoleRequest request);
    }
}
