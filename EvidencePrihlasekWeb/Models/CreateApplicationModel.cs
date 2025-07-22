using System.ComponentModel.DataAnnotations;

namespace EvidencePrihlasekWeb.Models
{
    public class CreateApplicationModel
    {
        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(50)]
        public string Email { get; set; } = string.Empty;

        [Required]
        public DateTime BirthDate { get; set; }

        [Required]
        [MaxLength(50)]
        public string Address { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string City { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string PostalCode { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Country { get; set; } = string.Empty;

        [Phone]
        public string? ContactPhone { get; set; }

        public string? AdditionalInfo { get; set; }

        public int? HighSchoolId1 { get; set; } = null;
        public int? HighSchoolId2 { get; set; } = null;
        public int? HighSchoolId3 { get; set; } = null;
        public int? StudyProgramId1 { get; set; } = null;
		public int? StudyProgramId2 { get; set; } = null;
        public int? StudyProgramId3 { get; set; } = null;
    }
}
