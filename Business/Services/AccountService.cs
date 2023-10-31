using Business.Interfaces;
using Common.Utilities;
using Communication.Interfaces;
using Data.Context;
using Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Models.Responses;
using Models.Responses.Accounts;
using Models.ServiceRequest;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Text;
using System.Text.Encodings.Web;
using static Models.Requests.Accounts.AccountRequest;

namespace Business.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<User> _userManager;
        private readonly ITokenService _tokenService;
        private readonly IEmailService _emailService;

        public AccountService(UserManager<User> userManager, ITokenService tokenService, IEmailService emailService)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _emailService = emailService;
        }

        public async Task<ApiResponse<AuthResponse>> ForgotPassword(string email)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                var authResponse = new AuthResponse();
                if (user == null)
                {
                    authResponse.Errors = new List<string>() { "User Not Found" };
                    return new ApiResponse<AuthResponse>
                    {
                        Result = authResponse,
                        Success = false
                    };
                }
                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                var callbackUrl = "http://localhost:5047/resetpassword?userId=" + user.Id + "&code=" + WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

                var emailFlag = await SendForgotEmail(user, callbackUrl);
                if (!emailFlag)
                {
                    return new ApiResponse<AuthResponse> { Result = null, ErrorMessage="Unable to send Forgot Email", Success = false };
                }
                return new ApiResponse<AuthResponse> { Result = null, Message = "Forgot Email sent", Success = true };

            }
            catch (Exception ex)
            {
                var authResponse = new AuthResponse() { Errors = new List<string> { ex.Message } };
                return new ApiResponse<AuthResponse>()
                {
                    Result = authResponse,
                    Success = false
                };
            }
        }

        

        public async Task<ApiResponse<AuthResponse>> Signup(SignupRequest request)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(request.Email);
                var authResponse = new AuthResponse();
                if(user != null)
                {
                    authResponse.Errors = new List<string>() { "Email address already used! try new email address" };
                    return new ApiResponse<AuthResponse>
                    {
                        Result = authResponse,
                        Success = false
                    };
                }
                var newUser = new User
                {
                    Email = request.Email,
                    UserName = request.Username,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    EmailConfirmed = false,
                    CreatedDate = DateTime.UtcNow,
                    IsActive = true,
                    IsDeleted = false
                };
                var isCreated = await _userManager.CreateAsync(newUser, request.Password);

                if (isCreated.Succeeded)
                {
                    //Assign the Employee Role
                    var result = await _userManager.AddToRoleAsync(newUser, "Employee");
                    if (result.Succeeded)
                    {
                        //var jwtToken = await _tokenService.GenerateJwtToken(newUser);
                        //send email for confirmation
                        //string message = string.Empty;
                        var code = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);
                        var callbackUrl = request.WebHostUrl + "accountactivation?userId=" + newUser.Id + "&code=" + WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

                        var emailFlag = await SendConfirmEmail(newUser, callbackUrl);
                        if (emailFlag)
                        {
                            return new ApiResponse<AuthResponse> { Result = null, Success = true };
                        }                       
                        
                    }
                    else
                    {
                        authResponse.Errors = isCreated.Errors.Select(x => x.Description).ToList();
                        return new ApiResponse<AuthResponse>()
                        {
                            Result = authResponse,
                            Success = false
                        };
                    }
                }
                else
                {
                    authResponse.Errors = isCreated.Errors.Select(x => x.Description).ToList();
                    return new ApiResponse<AuthResponse>()
                    {
                        Result = authResponse,
                        Success = false
                    };
                }

            }
            catch (Exception ex)
            {
                var authResponse = new AuthResponse() { Errors = new List<string> { ex.Message } };
                return new ApiResponse<AuthResponse>()
                {
                    Result = authResponse,
                    Success = false
                };
            }


            throw new NotImplementedException();
        }

        public async Task<ApiResponse<AuthResponse>> RefreshToken(TokenRequest tokenRequest)
        {
            var result = await _tokenService.VerifyAndGenerateToken(tokenRequest);
            return new ApiResponse<AuthResponse> { Result = result, Success = true };
        }

        private async Task<bool> SendConfirmEmail(User user, string url)
        {
            string message = string.Empty;
            message += "Hi " + user.FirstName + "<br /><br />";
            message += $"Please confirm your account by clicking <a href='{HtmlEncoder.Default.Encode(url)}'>here</a>.";
            message += "<br /><br />Team Pac HRMS";
            var mailRequest = new MailRequest() { Attachments = null, Body = message, Subject = "Pac HRMS - Verification Email", ToEmail = user.Email };
            return await _emailService.SendEmailAsync(mailRequest);
        }

        private async Task<bool> SendForgotEmail(User user, string url)
        {
            string message = string.Empty;
            message += "Hi " + user.FirstName + "<br /><br />";
            message += $"Please reset your account by clicking <a href='{HtmlEncoder.Default.Encode(url)}'>here</a>.";
            message += "<br /><br />Team Pac HRMS";
            var mailRequest = new MailRequest() { Attachments = null, Body = message, Subject = "Pac HRMS - Verification Email", ToEmail = user.Email };
            return await _emailService.SendEmailAsync(mailRequest);
        }
    }
}
