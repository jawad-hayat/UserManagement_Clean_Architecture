using Business.Interfaces;
using Data.Context;
using Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Models.ConfigModels;
using Models.Requests.Accounts;
using Models.Responses.Accounts;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static Models.Requests.Accounts.AccountRequest;
using static Common.Utilities.Utility;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using System.Net.Sockets;

namespace Business.Services
{
    public class TokenService : ITokenService
    {
        private readonly JwtConfig _jwtConfig;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly TokenValidationParameters _tokenValidationParameters;
        private readonly ApplicationDbContext _dbContext;
        private readonly IConfiguration _configuration;
        public TokenService(JwtConfig jwtConfig, UserManager<User> userManager, TokenValidationParameters tokenValidationParameters, ApplicationDbContext dbContext, RoleManager<IdentityRole> roleManager,IConfiguration configuration)
        {
            _jwtConfig = jwtConfig;
            _userManager = userManager;
            _tokenValidationParameters = tokenValidationParameters;
            _dbContext = dbContext;
            _roleManager = roleManager;
            _configuration = configuration;
        }
        public async Task<AuthResponse> GenerateJwtToken(User user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtConfig.Secret);
            var userRoles = await _userManager.GetRolesAsync(user);
            var role = await _roleManager.FindByNameAsync(userRoles[0]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("Id", user.Id),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(ClaimTypes.Role, userRoles[0]),
                    new Claim("RoleId", role.Id)
                }),
                Expires = DateTime.UtcNow.AddMinutes(Convert.ToInt32(_jwtConfig.ExpirationTime)),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = jwtTokenHandler.CreateToken(tokenDescriptor);
            var jwtToken = jwtTokenHandler.WriteToken(token);

            var refreshToken = new RefreshToken()
            {
                JwtId = token.Id,
                IsUsed = false,
                IsRevoked = false,
                ApplicationUserId = user.Id,
                CreatedDate = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddDays(Convert.ToInt32(_jwtConfig.RefreshTokenExpiration)),
                Token = RandomString(35) + Guid.NewGuid()
            };

            await _dbContext.RefreshTokens.AddAsync(refreshToken);
            await _dbContext.SaveChangesAsync();

            return new AuthResponse()
            {
                Token = jwtToken,
                Email = user.Email,
                UserId = user.Id,
                Name = user.FirstName + " " + user.LastName,
                Success = true,
                RefreshToken = refreshToken.Token
            };
        }

        public async Task<AuthResponse> VerifyAndGenerateToken(TokenRequest tokenRequest)
        {
            var key = _configuration.GetSection("JwtConfig").GetSection("Secret").Value;
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key)),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = false,
                RequireExpirationTime = false,
                ClockSkew = TimeSpan.Zero,
            };
            var jwtTokenHandler = new JwtSecurityTokenHandler();

            try
            {
                // Validation 1 - Validation JWT token format
                var tokenInVerification = jwtTokenHandler.ValidateToken(tokenRequest.Token, tokenValidationParameters, out var validatedToken);

                // Validation 2 - Validate encryption alg
                if (validatedToken is JwtSecurityToken jwtSecurityToken)
                {
                    var result = jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase);
                    if (!result)
                    {
                        return null;
                    }
                }

                // Validation 3 - validate expiry date
                var utcExpiryDate = long.Parse(tokenInVerification.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp).Value);
                var expiryDate = UnixTimeStampToDateTime(utcExpiryDate);
                if (expiryDate > DateTime.UtcNow)
                {
                    return new AuthResponse()
                    {
                        Success = false,
                        Errors = new List<string>() { "Token has not yet expired" }
                    };
                }

                // validation 4 - validate existence of the token
                var storedToken = await _dbContext.RefreshTokens.FirstOrDefaultAsync(x => x.Token == tokenRequest.RefreshToken);

                if (storedToken == null)
                {
                    return new AuthResponse()
                    {
                        Success = false,
                        Errors = new List<string>() { "Token does not exist" }
                    };
                }

                // Validation 5 - validate if used
                if (storedToken.IsUsed)
                {
                    return new AuthResponse()
                    {
                        Success = false,
                        Errors = new List<string>() { "Token has been used" }
                    };
                }

                // Validation 6 - validate if revoked
                if (storedToken.IsRevoked)
                {
                    return new AuthResponse()
                    {
                        Success = false,
                        Errors = new List<string>() { "Token has been revoked" }
                    };
                }

                // Validation 7 - validate the id
                var jti = tokenInVerification.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti).Value;

                if (storedToken.JwtId != jti)
                {
                    return new AuthResponse()
                    {
                        Success = false,
                        Errors = new List<string>() { "Token doesn't match" }
                    };
                }

                // update current token 
                storedToken.IsUsed = true;
                _dbContext.RefreshTokens.Update(storedToken);
                await _dbContext.SaveChangesAsync();

                // Generate a new token
                var dbUser = await _userManager.FindByIdAsync(storedToken.ApplicationUserId);
                return await GenerateJwtToken(dbUser);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Lifetime validation failed. The token is expired."))
                {
                    return new AuthResponse()
                    {
                        Success = false,
                        Errors = new List<string>() { "Token has expired please re-login" }
                    };
                }
                else
                {
                    return new AuthResponse()
                    {
                        Success = false,
                        Errors = new List<string>() { "Something went wrong." }
                    };
                }
            }
        }        
    }
}
