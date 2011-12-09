using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Belgian_Cinema.Classes;
using Belgian_Cinema.Model;
using Microsoft.Phone.Controls;

namespace Belgian_Cinema
{
    public partial class settingsPage : PhoneApplicationPage
    {

        List<Cinema> cinemas;
        AppSettings appSettings= new AppSettings();
        public settingsPage()
        {
            InitializeComponent();
            FillCinema();
            listPickerCinemas.ItemsSource = cinemas;
            listPickerCinemas.DisplayMemberPath = "CinemaName";

            listPickerLanguage.Items.Add("French");
            listPickerLanguage.Items.Add("Dutch");
            
        }

        private void FillCinema()
        {
            //Todo : parse thse from website
            cinemas = (App.Current as App).bigCinemaList;
        }

        private void listPickerCinemas_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
         //   (App.Current as App).selectedCinema = (Cinema)listPickerCinemas.SelectedItem;
        }
        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            appSettings.CinemaSetting = (Cinema)listPickerCinemas.SelectedItem;


            if (listPickerLanguage.SelectedItem.ToString() == "French")
                appSettings.LanguageSetting = "fr";
            else
            {
                appSettings.LanguageSetting = "nl";
            }

            base.OnNavigatedFrom(e);
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            for (int i = 0; i < listPickerCinemas.Items.Count; i++)
            {
                if(((Cinema)listPickerCinemas.Items[i]).CinemaId==((Cinema)appSettings.CinemaSetting).CinemaId)
                {
                    listPickerCinemas.SelectedIndex = i;
                    break;
                }
            }


            string lang = "French";
            if (appSettings.LanguageSetting != "fr")
                lang = "Dutch";

            for (int i = 0; i < listPickerCinemas.Items.Count; i++)
            {
                if (((string)listPickerLanguage.Items[i]) == lang)
                {
                    listPickerLanguage.SelectedIndex = i;
                    break;
                }
            }
            
        }

    }
}