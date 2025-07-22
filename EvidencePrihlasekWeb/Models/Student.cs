namespace EvidencePrihlasekWeb.Models
{
    public class Student
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        public int UserId { get; set; }
        public string Address { get; set; } = string.Empty;

        public string City { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string ContactPhone { get; set; } = string.Empty;
        public string AdditionalInfo { get; set; } = string.Empty;
        public DateTime BirthDate { get; set; }
        public string Email { get; set; } = string.Empty;
    }
}
