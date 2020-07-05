using System.ComponentModel.DataAnnotations;

namespace Outsourcing.Web.Models.Account
{
    public class LoginViewModel
    {
        [Required]
        public string UsernameOrEmailAddress { get; set; }

        [Required]
        public string Password { get; set; }
        public string Language { get; set; }
        public bool RememberMe { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}