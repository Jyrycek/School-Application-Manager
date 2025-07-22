using EvidencePrihlasek.Classes;
using EvidencePrihlasek.Model;
using System.Windows;
using System.Windows.Controls;

namespace EvidencePrihlasekDesktop.View
{
	/// <summary>
	/// Interakční logika pro EditApplicationWindow.xaml
	/// </summary>
	public partial class EditApplicationWindow : Window
	{
		private readonly ApplicationListModel applicationList;
		public EditApplicationWindow(ApplicationListModel _applicationList)
		{
			InitializeComponent();

			applicationList = _applicationList;

			DataContext = applicationList;
		}

		private void ComboBox_School_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			ComboBox comboBox = sender as ComboBox;
			if (comboBox != null)
			{
				HighSchool selectedSchool = comboBox.SelectedItem as HighSchool;
				if (selectedSchool != null)
				{

					if (comboBox.Name == "comboBox1")
					{
						applicationList.SelectedSchool1 = selectedSchool;
						applicationList.SelectedSchool1.StudyPrograms = selectedSchool.StudyPrograms;
						comboBoxSt1.ItemsSource = selectedSchool.StudyPrograms;
					}
					else if (comboBox.Name == "comboBox2")
					{
						applicationList.SelectedSchool2 = selectedSchool;
						applicationList.SelectedSchool2.StudyPrograms = selectedSchool.StudyPrograms;
						comboBoxSt2.ItemsSource = selectedSchool.StudyPrograms;
					}
					else if (comboBox.Name == "comboBox3")
					{
						applicationList.SelectedSchool3 = selectedSchool;
						applicationList.SelectedSchool3.StudyPrograms = selectedSchool.StudyPrograms;
						comboBoxSt3.ItemsSource = selectedSchool.StudyPrograms;
					}
				}
			}
		}

		private async void SaveChanges_Click(object sender, RoutedEventArgs e)
		{
			if (comboBoxSt1.SelectedItem == comboBoxSt2.SelectedItem && comboBoxSt1.SelectedItem != null
				|| comboBoxSt1.SelectedItem == comboBoxSt3.SelectedItem && comboBoxSt1.SelectedItem != null
				|| comboBoxSt2.SelectedItem == comboBoxSt3.SelectedItem && comboBoxSt2.SelectedItem != null)
			{
				MessageBox.Show("Nemůžeš vybrat jeden studijní program vícekrát", "Chyba");
				return;
			}

			if (comboBox1.SelectedItem != null && comboBoxSt1.SelectedItem == null ||
				comboBox2.SelectedItem != null && comboBoxSt2.SelectedItem == null ||
				comboBox3.SelectedItem != null && comboBoxSt3.SelectedItem == null)
			{
				MessageBox.Show("Nebyly vybrány studijní programy", "Chyba");
				return;
			}
			else
			{
				await applicationList.UpdateApplicationStudent();
			}

			this.Close();
		}
    }
}