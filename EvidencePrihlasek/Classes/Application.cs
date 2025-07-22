using EvidencePrihlasek.Model;

namespace EvidencePrihlasek.Classes
{
    public class Application : ObsObject
    {
        public int Id { get; set; }
        private DateTime submissionDate;
        public int StudentId { get; set; }
        public int StudyProgramId1 { get; set; }
        public int StudyProgramId2 { get; set; }
        public int StudyProgramId3 { get; set; }
        public DateTime SubmissionDate
        {
            get => submissionDate.Date;
            set => submissionDate = value.Date;
        }
    }
}
