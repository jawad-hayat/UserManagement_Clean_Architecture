using Business.Interfaces;
using Data.Context;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Models.Responses;
using Models.Responses.Accounts;
using Org.BouncyCastle.Asn1.Ocsp;
using System.Net;
using System.Text;
using static Models.Requests.Accounts.AccountRequest;

namespace Assignment1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly UserManager<User> _userManager;
        

        public AccountController(IAccountService accountService, UserManager<User> userManager)
        {
            _accountService = accountService;
            _userManager = userManager;            
        }
        [HttpPost]
        [Route("signup")]
        public async Task<IActionResult> Signup([FromBody] SignupRequest request)
        {
            if (ModelState.IsValid)
            {
                var result = await _accountService.Signup(request);
                return new OkObjectResult(result);
            }
            return new OkObjectResult(new AuthResponse()
            {
                Errors = new List<string>() {
                        "will modify error!"
                    },
                Success = false
            });
        }
        [HttpGet]
        [Route("accountactivation")]
        public async Task<IActionResult> AccountActivation(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return BadRequest("please provide userid and token!");
                // report an error somehow
            }
            // check if token OK
            User user = await _userManager.FindByIdAsync(userId);
            if(user == null)
            {
                return BadRequest("user not found!");
            }
            IdentityResult result = await _userManager.ConfirmEmailAsync(user, Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code)));
            if (!result.Succeeded)
            {
                return BadRequest("error in user verification!");
            }
            return Ok("user verification successfully!");
        }

        [HttpGet]
        [Route("resetpassword")]
        public async Task<IActionResult> ResetPassword(string userId, string code, string newPassword)
        {
            if (userId == null || code == null)
            {
                return BadRequest("please provide userid and token!");
                // report an error somehow
            }
            // check if token OK
            User user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return BadRequest("user not found!");
            }
            IdentityResult result = await _userManager.ResetPasswordAsync(user, Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code)),newPassword);
            if (!result.Succeeded)
            {
                return BadRequest("error in user verification!");
            }
            return Ok("password changed successfully!");
        }

        [HttpGet]
        [Route("forgot_password")]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            if (email == null)
            {
                return BadRequest("please provide user email!");
                // report an error somehow
            }
            User user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return BadRequest("user not found!");
            }
            var emailFlag = await _accountService.ForgotPassword(user.Email);
            return Ok(emailFlag);
        }

        [HttpPost]
        [Route("refreshtoken")]
        public async Task<IActionResult> RefreshToken([FromBody] TokenRequest request)
        {
            if (ModelState.IsValid)
            {
                var result = await _accountService.RefreshToken(request);
                return new OkObjectResult(result);
            }
            return new OkObjectResult(new AuthResponse() { Errors = new List<string>() { "Invalid payload" }, Success = false });
        }

    }
}
