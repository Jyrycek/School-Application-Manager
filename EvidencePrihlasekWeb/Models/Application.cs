namespace EvidencePrihlasekWeb.Models
{
    public class Application
    {
        public DateTime SubmissionDate { get; set; }
        public int StudentId { get; set; }
        public int? StudyProgramId1 { get; set; }
        public int? StudyProgramId2 { get; set; }
        public int? StudyProgramId3 { get; set; }
    }
}
