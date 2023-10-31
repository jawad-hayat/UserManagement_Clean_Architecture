
using Domain.Models;
using Models.Responses.Accounts;
using static Models.Requests.Accounts.AccountRequest;

namespace Business.Interfaces
{
    public interface ITokenService
    {
        Task<AuthResponse> GenerateJwtToken(User user);
        Task<AuthResponse> VerifyAndGenerateToken(TokenRequest tokenRequest);
    }
}
