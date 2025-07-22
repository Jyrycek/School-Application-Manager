using System.Windows;
using System.Windows.Controls;
using EvidencePrihlasek.Classes;
using EvidencePrihlasek.EventHandlers;
using EvidencePrihlasek.Model;

namespace EvidencePrihlasek.View
{
    /// <summary>
    /// Interakční logika pro EditWindow.xaml
    /// </summary>
    public partial class EditWindow : Window
    {
        private SchoolListModel schoolList;
        public EditWindow(HighSchool highSchool, SchoolListModel schoolListModel)
        {
            InitializeComponent();

            schoolList = schoolListModel;
            DataContext = highSchool;
        }

        private async void Save_Selected(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            HighSchool hs = (HighSchool)button.DataContext;

            await schoolList.EditExistingSchoolAsync(hs);

            this.Close();
        }

        private void Button_EditProgramClick(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            HighSchool hs = (HighSchool)btn.DataContext;

            StudyProgram? selectedProgram = comboBoxPrograms.SelectedItem as StudyProgram;

            if (selectedProgram == null)
            {
                MessageBox.Show("Zvol si obor!", "Chyba");
                return;
            }

            ButtonEventHandler.BlockButton(btn, () => new EditStudyProgramWindow(selectedProgram));
        }
        public void EditStudyProgram(StudyProgram studyProgram)
        {
            if (studyProgram != null)
            {
                ((HighSchool)DataContext).StudyPrograms.Add(studyProgram);
            }
        }

        private void Button_AddProgramClick(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;

            ButtonEventHandler.BlockButton(btn, () => new AddStudyProgramWindow(this));
        }
    }
}
