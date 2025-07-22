using System.Collections.ObjectModel;
using System.Windows;
using EvidencePrihlasek.Classes;
using Database;

namespace EvidencePrihlasek.Model
{
    public class SchoolListModel : ObsObject
    {
        public ObservableCollection<HighSchool> Schools { get; set; }
        public RelayCommand RemoveSchoolCommand { get; set; }

        public SchoolListModel()
        {
            LoadDataAsync();

            RemoveSchoolCommand = new RelayCommand(RemoveSchool);
        }
        private async void RemoveSchool(object obj)
        {
            if (obj is HighSchool schoolToRemove)
            {
                bool successfull = await DatabaseManager.RemoveSchoolAsync<HighSchool>(schoolToRemove);

                if (successfull)
                {
					Schools.Remove(schoolToRemove);
				}
                else
                {
					MessageBox.Show("Nepodařilo se odstranit školu!", "Chyba");
				}
            }
            else
            {
                MessageBox.Show("Nepodařilo se odstranit školu!", "Chyba");
            }
        }

        private async void LoadDataAsync()
        {
            List<HighSchool> schoolsList = await DatabaseManager.GetAllAsync<HighSchool>();
            List<StudyProgram> studyPrograms = await DatabaseManager.GetAllAsync<StudyProgram>();

            Schools = AssignStudyProgramsToSchools(schoolsList, studyPrograms);
        }
        public async Task AddNewSchoolAsync(object obj)
        {
            if (obj is HighSchool schoolToAdd)
            {
                bool successfull = await DatabaseManager.AddSchoolAsync(schoolToAdd);

				if (successfull)
				{
					Schools.Add(schoolToAdd);
				}
				else
				{
					MessageBox.Show("Nepodařilo se odstranit školu!", "Chyba");
				}
			}
            else
            {
                MessageBox.Show("Nepodařilo se přidat školu!", "Chyba");
            }
        }
        public async Task EditExistingSchoolAsync(object obj)
        {
            if (obj is HighSchool schoolToEdit)
            {
				bool successfull = await DatabaseManager.EditSchoolAsync(schoolToEdit);


                if (successfull)
                {
					Schools.Remove(schoolToEdit);
					Schools.Add(schoolToEdit);
				} else
                {

                }
            }
            else
            {
                MessageBox.Show("Nepodařilo se přidat školu!", "Chyba");
            }
        }

        public static ObservableCollection<HighSchool> AssignStudyProgramsToSchools(List<HighSchool> schoolsList, List<StudyProgram> studyPrograms)
        {
            Dictionary<int, HighSchool> highSchoolList = schoolsList.ToDictionary(program => program.Id);

            foreach (StudyProgram studyProgram in studyPrograms)
            {
                if (highSchoolList.ContainsKey(studyProgram.HighSchoolId))
                {
                    HighSchool school = highSchoolList[studyProgram.HighSchoolId];
                    
                    school.StudyPrograms.Add(studyProgram);
                }
            }

            return new ObservableCollection<HighSchool>(highSchoolList.Values);
        }
    }
}