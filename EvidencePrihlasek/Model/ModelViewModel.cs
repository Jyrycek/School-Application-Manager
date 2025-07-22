using EvidencePrihlasek.Classes;
using EvidencePrihlasek.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace EvidencePrihlasek.Model
{
    public class ModelViewModel : ObsObject
    {
        public RelayCommand HomeCommand { get; set; }
        public RelayCommand AboutAppCommand { get; set; }
        public RelayCommand SchoolListCommand { get; set; }
        public RelayCommand ApplicationListCommand { get; set; }

        public HomeModel Home { get; set; }
        public ApplicationListModel applicationList { get; set; }
        public SchoolListModel SchoolList { get; set; }
        public AboutAppModel AboutApp { get; set; }

        private object currentView;
        public object CurrentView
        {
            get => currentView;
            set
            {
                currentView = value;
                OnPropertyChanged();
            }
        }

        public ModelViewModel() 
        {
            this.Home = new HomeModel();
            this.SchoolList = new SchoolListModel();
            this.AboutApp = new AboutAppModel();
            this.applicationList = new ApplicationListModel();

            CurrentView = Home;

            HomeCommand = new RelayCommand(o => {
                CurrentView = Home;
            });
            ApplicationListCommand = new RelayCommand(o => {
                CurrentView = applicationList;
            });
            SchoolListCommand = new RelayCommand(o => {
                CurrentView = SchoolList;
            });
            AboutAppCommand = new RelayCommand(o =>
            {
                CurrentView = AboutApp;
            });

        }
    }
}
