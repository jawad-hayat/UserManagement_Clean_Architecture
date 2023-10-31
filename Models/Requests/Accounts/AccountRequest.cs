using System.ComponentModel.DataAnnotations;

namespace Models.Requests.Accounts
{
    public class AccountRequest
    {
        public class SignInRequest
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }
            [Required]
            public string Password { get; set; }
        }

        public class SignupRequest
        {
            [Required]
            public string Username { get; set; }
            [Required]
            [EmailAddress]
            public string Email { get; set; }
            [Required]
            public string Password { get; set; }
            [Required]
            public string FirstName { get; set; }
            [Required]
            public string LastName { get; set; }
            public DateTime CreatedDate { get; set; }
            public string WebHostUrl { get; set; } = "http://localhost:5047/";
        }

        public class TokenRequest
        {
            [Required]
            public string Token { get; set; }

            [Required]
            public string RefreshToken { get; set; }
        }

    }
}
