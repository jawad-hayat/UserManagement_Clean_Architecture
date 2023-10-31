using Business.Interfaces;
using Domain.Models;
using Microsoft.AspNetCore.Identity;
using Models.Responses;
using Models.Responses.Accounts;
using static Models.Requests.Accounts.AccountRequest;

namespace Business.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly ITokenService _tokenService;
        public AuthService(UserManager<User> userManager, ITokenService tokenService)
        {
            _userManager = userManager;
            _tokenService = tokenService;
        }
        
        public async Task<ApiResponse<AuthResponse>> Login(SignInRequest request)
        {
            var userExist = await _userManager.FindByEmailAsync(request.Email);

            var authResponse = new AuthResponse();
            if (userExist == null)
            {
                authResponse.Errors = new List<string>() { "User not found" };
                return new ApiResponse<AuthResponse>()
                {
                    Result = authResponse,
                    Success = false
                };
            }

            var isValid = await _userManager.CheckPasswordAsync(userExist, request.Password);

            if (!isValid)
            {
                authResponse.Errors = new List<string>() { "Invalid login request" };
                return new ApiResponse<AuthResponse>()
                {
                    Result = authResponse,
                    Success = false
                };
            }
            if (!userExist.EmailConfirmed)
            {
                authResponse.Errors = new List<string>() { "Your email is not confirmed, pleaes check your email to confirm your identity" };
                return new ApiResponse<AuthResponse>()
                {
                    Result = authResponse,
                    Success = false
                };
            }

            var jwtToken = await _tokenService.GenerateJwtToken(userExist);
            return new ApiResponse<AuthResponse>
            {
                Success = true,
                Result = jwtToken
            };
        }
    }
}
