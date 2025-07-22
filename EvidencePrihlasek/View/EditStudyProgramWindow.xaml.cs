using EvidencePrihlasek.Classes;
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

namespace EvidencePrihlasek.View
{
    /// <summary>
    /// Interakční logika pro EditStudyProgramWindow.xaml
    /// </summary>
    public partial class EditStudyProgramWindow : Window
    {
        public event EventHandler ProgramEdited;
        public EditStudyProgramWindow(StudyProgram studyProgram)
        {
            InitializeComponent();

            DataContext = studyProgram;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
