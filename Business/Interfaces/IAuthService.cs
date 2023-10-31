using Models.Responses;
using Models.Responses.Accounts;
using static Models.Requests.Accounts.AccountRequest;

namespace Business.Interfaces
{
    public interface IAuthService
    {
        Task<ApiResponse<AuthResponse>> Login(SignInRequest request);
    }
}
