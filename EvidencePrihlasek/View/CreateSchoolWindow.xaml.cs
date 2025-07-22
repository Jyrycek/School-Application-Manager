using EvidencePrihlasek.Model;
using System.Windows;
using System.Windows.Controls;
using EvidencePrihlasek.Classes;
using EvidencePrihlasek.EventHandlers;

namespace EvidencePrihlasek.View
{
    /// <summary>
    /// Interakční logika pro CreateSchoolWindow.xaml
    /// </summary>
    public partial class CreateSchoolWindow : Window
    {
        private SchoolListModel schoolList;
        public CreateSchoolWindow(SchoolListModel schoolmodel)
        {
            InitializeComponent();

            schoolList = schoolmodel;
            DataContext = new HighSchool();
        }

        private async void Button_AddSchool(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            HighSchool hs = (HighSchool)button.DataContext;

            await schoolList.AddNewSchoolAsync(hs);

            this.Close();
        }

        private void Button_AddProgramClick(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;

            ButtonEventHandler.BlockButton(button, () => new AddStudyProgramWindow(this));
        }
        public void AddStudyProgram(StudyProgram studyProgram)
        {
            if (studyProgram != null)
            {
                ((HighSchool)DataContext).StudyPrograms.Add(studyProgram);
            }
        }
    }
}
