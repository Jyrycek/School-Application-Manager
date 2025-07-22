using EvidencePrihlasek.Classes;
using System.Reflection;
using System.Text;
using System.Windows;
using Database;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using EvidencePrihlasek.View;

namespace EvidencePrihlasek.Model
{
	public class ApplicationListModel : ObsObject
	{
		public RelayCommand ShowStudentCommand { get; set; }
		public RelayCommand ShowApplicationCommand { get; set; }
		public RelayCommand Show { get; set; }
		private Student selectedStudent;
		public Student SelectedStudent
		{
			get => selectedStudent;
			set => SetProperty(ref selectedStudent, value);
		}
		private string studentDetails;
		public string StudentDetails
		{
			get => studentDetails;
			set => SetProperty(ref studentDetails, value);
		}
		private string schoolDetails;
		public string SchoolDetails
		{
			get => schoolDetails;
			set => SetProperty(ref schoolDetails, value);
		}
		private HighSchool selectedSchool1;
		public HighSchool SelectedSchool1
		{
			get => selectedSchool1;
			set => SetProperty(ref selectedSchool1, value);
		}
		private HighSchool selectedSchool2;
		public HighSchool SelectedSchool2
		{
			get => selectedSchool2;
			set => SetProperty(ref selectedSchool2, value);
		}
		private HighSchool selectedSchool3;
		public HighSchool SelectedSchool3
		{
			get => selectedSchool3;
			set => SetProperty(ref selectedSchool3, value);
		}
		private StudyProgram selectedStudyProgram1 = new StudyProgram();
		public StudyProgram SelectedStProgram1
		{
			get => selectedStudyProgram1;
			set => SetProperty(ref selectedStudyProgram1, value);
		}
		private StudyProgram selectedStudyProgram2 = new StudyProgram();
		public StudyProgram SelectedStProgram2
		{
			get => selectedStudyProgram2;
			set => SetProperty(ref selectedStudyProgram2, value);
		}
		private StudyProgram selectedStudyProgram3 = new StudyProgram();
		public StudyProgram SelectedStProgram3
		{
			get => selectedStudyProgram3;
			set => SetProperty(ref selectedStudyProgram3, value);
		}

		public ObservableCollection<Classes.Application> StudentApplications { get; set; }
		public ObservableCollection<Student> Students { get; set; }
		public ObservableCollection<HighSchool> Schools { get; set; }

		public ApplicationListModel()
		{
			LoadDataAsync();

			ShowStudentCommand = new RelayCommand(ShowStudent);
			ShowApplicationCommand = new RelayCommand(ShowSchools);
		}
		private async void LoadDataAsync()
		{
			Students = new ObservableCollection<Student>(await DatabaseManager.GetAllAsync<Student>());
			List<HighSchool> schoolsList = await DatabaseManager.GetAllAsync<HighSchool>();
			List<StudyProgram> studyPrograms = await DatabaseManager.GetAllAsync<StudyProgram>();

			StudentApplications = new ObservableCollection<Classes.Application>(await DatabaseManager.GetAllAsync<Classes.Application>());

			Schools = SchoolListModel.AssignStudyProgramsToSchools(schoolsList, studyPrograms);
		}

		private void ShowStudent(object obj)
		{
			if (obj is Student student)
			{
				string birthDateString = student.BirthDate.Date != DateTime.MinValue ? student.BirthDate.Date.ToShortDateString() : "Nezadáno";

				StudentDetails = $"Jméno: {student.FirstName}\n" +
								 $"Příjmení: {student.LastName}\n" +
								 $"Datum narození: {birthDateString}\n" +
								 $"Adresa: {student.Address}\n" +
								 $"Město: {student.City}\n" +
								 $"PSČ: {student.PostalCode}\n" +
								 $"Země: {student.Country}\n" +
								 $"Kontaktní telefon: {student.ContactPhone}\n" +
								 $"Email: {student.Email}\n" +
								 $"Další informace: {(string.IsNullOrEmpty(student.AdditionalInfo) ? "Neznámý" : student.AdditionalInfo)}";
			}
		}
		private void ShowSchools(object obj)
		{
			if (obj is Student student && student != null)
			{
				Classes.Application? studentApplicationInfo = StudentApplications.FirstOrDefault(app => app.StudentId == student.Id);
				string applicationInfo = "";
				if (studentApplicationInfo == null) { return; }

				applicationInfo = $"ID: {studentApplicationInfo.Id}\n" +
								  $"Datum podání přihlášky: {studentApplicationInfo.SubmissionDate.Date.ToShortDateString()}\n\n";

				StringBuilder schoolInfoBuilder = new StringBuilder();

				getStudyProgram(ref schoolInfoBuilder, studentApplicationInfo);

				SchoolDetails = applicationInfo + schoolInfoBuilder.ToString();
			}
		}
		private void getStudyProgram(ref StringBuilder schoolInfoBuilder, Classes.Application application)
		{
			Type type = application.GetType();

			for (int i = 1; i <= 3; i++)
			{
				string propertyName = "StudyProgramId" + i.ToString();
				PropertyInfo propertyInfo = type.GetProperty(propertyName);

				int id = (int)propertyInfo.GetValue(application, null);

				schoolInfoBuilder.AppendLine($"\nStudijní program č. {i}\n");

				StudyProgram? studyProgram = new StudyProgram();
				HighSchool? highSchool = new HighSchool();

				foreach (var school in Schools)
				{
					studyProgram = school.StudyPrograms.FirstOrDefault(p => p.Id == id);
					if (studyProgram != null)
					{
						highSchool = school;

						break;
					}
				}

				if (highSchool.Id == 0)
				{
					highSchool = null;
				}

				if (i == 1)
				{
					SelectedSchool1 = highSchool;
					SelectedStProgram1 = studyProgram;
				}
				else if (i == 2)
				{
					SelectedSchool2 = highSchool;
					SelectedStProgram2 = studyProgram;
				}
				else if (i == 3)
				{
					SelectedSchool3 = highSchool;
					SelectedStProgram3 = studyProgram;
				}

				if(studyProgram == null)
				{
					schoolInfoBuilder.AppendLine($"Nevybráno\n");
					continue;
				}

				schoolInfoBuilder.AppendLine($"Škola: {highSchool.Name}\n" +
											 $"Studijní program: {studyProgram.Name}\n" +
											 $"Popis: {studyProgram.Description}\n" +
											 $"Počet otevřených míst: {studyProgram.OpenSlots}\n");
			}
		}
		public async Task RemoveApplicationStudent(Student student)
		{
			if (student != null)
			{
				int studentId = student.Id;
				bool success = await DatabaseManager.RemoveStudentAndApplicationsAsync(studentId);

				if (success)
				{
					StudentApplications.Remove(StudentApplications.First(x => x.StudentId == studentId));
					Students.Remove(student);

					MessageBox.Show("Příhláška byla úšpěšně odstraněna", "Úspěch");
				}
				else
				{
					MessageBox.Show("Nepodařilo se odstranit přihlášku!", "Chyba");
				}
			}
			else
			{
				MessageBox.Show("Nebyl vybrán žádný student!", "Chyba");
			}
		}

		public async Task UpdateApplicationStudent()
		{
			Student st = new Student();
			st = this.selectedStudent;

			Classes.Application app = new Classes.Application();
			app = this.StudentApplications.First(x => x.StudentId == st.Id);
			app.StudyProgramId1 = this.selectedStudyProgram1.Id;
			app.StudyProgramId2 = this.selectedStudyProgram2.Id;
			app.StudyProgramId3 = this.selectedStudyProgram3.Id;

			await DatabaseManager.UpdateAsync<Student>(st);
			await DatabaseManager.UpdateAsync<Classes.Application>(app);

			MessageBox.Show("Příhláška byla úšpěšně upravena", "Úspěch");
		}
	}
}
