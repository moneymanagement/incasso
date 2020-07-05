using System.ComponentModel.DataAnnotations;

namespace Collection.Web.Models.Account
{
    public class LoginViewModel
    {
        [Required]
        public string UsernameOrEmailAddress { get; set; }

        [Required]
        public string Password { get; set; }
        public string Language { get; set; }

        public bool RememberMe { get; set; }
    }
}