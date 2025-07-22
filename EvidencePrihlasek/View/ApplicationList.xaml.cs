using EvidencePrihlasek.Classes;
using EvidencePrihlasek.EventHandlers;
using EvidencePrihlasek.Model;
using EvidencePrihlasekDesktop.View;
using System;
using System.Windows;
using System.Windows.Controls;

namespace EvidencePrihlasek.View
{
    /// <summary>
    /// Interakční logika pro ApplicationList.xaml
    /// </summary>
    public partial class ApplicationList : UserControl
    {
        private readonly ApplicationListModel applicationList;

        public ApplicationList()
        {
            InitializeComponent();

            applicationList = new ApplicationListModel();
            DataContext = applicationList;
        }

        private async void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Student selectedStudent = (sender as ListView).SelectedItem as Student;

            applicationList.SelectedStudent = selectedStudent;

			applicationList.ShowStudentCommand.Execute(selectedStudent);
            applicationList.ShowApplicationCommand.Execute(selectedStudent);
        }

		private async void Button_RemoveApplication_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			ApplicationListModel applicationListModel = DataContext as ApplicationListModel;

			Student selectedStudent = applicationListModel.SelectedStudent;

			await applicationList.RemoveApplicationStudent(selectedStudent);
		}

		private void Button_EditAppClick(object sender, System.Windows.RoutedEventArgs e)
		{
			ApplicationListModel applicationListModel = DataContext as ApplicationListModel;
			if (applicationListModel.StudentDetails != null)
			{
				ButtonEventHandler.BlockButton((Button)sender, () => new EditApplicationWindow(applicationList));
			}else
			{
				MessageBox.Show("Nebyla zvolena přihláška", "Chyba");
			}
		}
	}
}
