using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace EvidencePrihlasekWeb.Models
{
    public class Users
    {
        public int Id { get; set; }
        [Required]
        [MinLength(4)]
        [MaxLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [MinLength(4)]
        [MaxLength(50)]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        [MinLength(4)]
        [MaxLength(50)]
        [JsonIgnore]
        public string Password2 { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }
}
