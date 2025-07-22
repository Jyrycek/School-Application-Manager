using EvidencePrihlasek.Model;

namespace EvidencePrihlasek.Classes
{
    public class StudyProgram
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Identifier { get; set; }
        public string Description { get; set; }
        public int OpenSlots { get; set; }
        public int HighSchoolId { get; set; }
    }
}
