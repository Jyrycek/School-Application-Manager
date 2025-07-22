namespace EvidencePrihlasekWeb.Models
{
    public class DashboardModel
    {
        public bool ApplicationSubmitted { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateTime BirthDate { get; set; }
		public Tuple<HighSchool, StudyProgram> SchoolAndProgram1 { get; set; } = null!;
        public Tuple<HighSchool, StudyProgram> SchoolAndProgram2 { get; set; } = null!;
		public Tuple<HighSchool, StudyProgram> SchoolAndProgram3 { get; set; } = null!;
		public DateTime SubmissionDate { get; set; } = DateTime.Now;
        public string ContactPhone { get; set; } = string.Empty;
    }

}
