using EvidencePrihlasek.Model;
using System.Windows;

namespace EvidencePrihlasek
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new ModelViewModel();
        }
    }
}