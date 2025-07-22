using EvidencePrihlasek.Model;
using System.Windows;
using System.Windows.Controls;
using EvidencePrihlasek.Classes;
using EvidencePrihlasek.EventHandlers;

namespace EvidencePrihlasek.View
{
    /// <summary>
    /// Interakční logika pro SchoolList.xaml
    /// </summary>
    public partial class SchoolList : UserControl
    {
        private SchoolListModel viewModel;
        public SchoolList()
        {
            InitializeComponent();

            viewModel = new SchoolListModel();
            DataContext = viewModel;
        }
        private void DataGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit && e.Row.Item is HighSchool newSchool)
            {
                HighSchool highSchool =  viewModel.Schools.First(school =>  school.Id == newSchool.Id);
                newSchool.Id = highSchool.Id;
                
                viewModel.Schools.Remove(highSchool);
                viewModel.Schools.Add(newSchool);
            }
        }
        private void Button_AddSchool(object sender, RoutedEventArgs e)
        {
            ButtonEventHandler.BlockButton((Button)sender, () => new CreateSchoolWindow(viewModel));
        }
        private void Button_EditSchool(object sender, RoutedEventArgs e)
        {
            Button btw = (Button)sender;
            HighSchool hs = (HighSchool)btw.DataContext;

            ButtonEventHandler.BlockButton(btw, () => new EditWindow(hs, viewModel));
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            if (comboBox != null)
            {
                comboBox.SelectedIndex = -1;
            }
        }
    }
}
