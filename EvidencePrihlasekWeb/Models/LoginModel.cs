using System.ComponentModel.DataAnnotations;

namespace EvidencePrihlasekWeb.Models
{
    public class LoginModel
    {
        [Required]
        [MinLength(4)]
        [MaxLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [MinLength(4)]
        [MaxLength(50)]
        public string Password { get; set; } = string.Empty;
    }
}
