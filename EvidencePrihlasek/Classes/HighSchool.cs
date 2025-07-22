using EvidencePrihlasek.Model;
using System.Collections.ObjectModel;
using System.Windows;
using System.Text.Json.Serialization;

namespace EvidencePrihlasek.Classes
{
    public class HighSchool
    {
        public int Id { get; set; }
        public string Name { get; set; }
		public string Address { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public string ContactPhone { get; set; }
        public string Web { get; set; }
        public string? Info { get; set; }
        [JsonIgnore]
        public ObservableCollection<StudyProgram> StudyPrograms { get; set; }

        public HighSchool() 
        {
            this.StudyPrograms = new ObservableCollection<StudyProgram>();
        }
    }
}
