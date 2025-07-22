using EvidencePrihlasek.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using EvidencePrihlasek.Classes;

namespace EvidencePrihlasek.View
{
    /// <summary>
    /// Interakční logika pro AddStudyProgramWindow.xaml
    /// </summary>
    public partial class AddStudyProgramWindow : Window
    {
        private CreateSchoolWindow createSchoolWindow;
        private EditWindow editWindow;
        public AddStudyProgramWindow(CreateSchoolWindow _createSchoolWindow)
        {
            InitializeComponent();

            DataContext = new StudyProgram();
            createSchoolWindow = _createSchoolWindow;
        }
        public AddStudyProgramWindow(EditWindow _editWindow)
        {
            InitializeComponent();

            DataContext = new StudyProgram();
            editWindow = _editWindow;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            StudyProgram studyProgram = (StudyProgram)DataContext;

            if (createSchoolWindow != null)
            {
                createSchoolWindow.AddStudyProgram(studyProgram);
            }
            else
            {
                editWindow.EditStudyProgram(studyProgram);
            }

            this.Close();
        }
    }
}
