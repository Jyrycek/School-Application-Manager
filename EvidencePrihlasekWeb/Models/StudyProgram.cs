namespace EvidencePrihlasekWeb.Models
{
    public class StudyProgram
    {
        public int Id { get; set; }
        public string Name { get; set; } = "Nezvoleno";
        public string Identifier { get; set; } = "Nezvoleno";
        public int OpenSlots { get; set; } = 0;
        public string Description { get; set; } = "Nezvoleno";
        public int HighSchoolId { get; set; }
    }
}
