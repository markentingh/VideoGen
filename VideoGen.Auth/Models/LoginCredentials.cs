using System.ComponentModel.DataAnnotations;

namespace VideoGen.Auth.Models
{
    public class LoginCredentials
    {
        [Required]
        public string Username { get; set; } = "";

        [Required]
        public string Password { get; set; } = "";
    }
}
