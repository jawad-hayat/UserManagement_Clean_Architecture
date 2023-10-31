using Models.Responses.Accounts;
using Models.Responses;
using static Models.Requests.Accounts.AccountRequest;

namespace Business.Interfaces
{
    public interface IAccountService
    {
        Task<ApiResponse<AuthResponse>> Signup(SignupRequest request);
        Task<ApiResponse<AuthResponse>> ForgotPassword(string email);
        Task<ApiResponse<AuthResponse>> RefreshToken(TokenRequest tokenRequest);
    }
}
