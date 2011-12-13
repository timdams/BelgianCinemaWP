using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Belgian_Cinema.Model;
using Belgian_Cinema.UtilityClasses;
using WP7Contrib.View.Transitions.Animation;

namespace Belgian_Cinema
{
    public partial class settingsPage : AnimatedBasePage
    {

        ObservableCollection<Cinema> cinemas;
        AppSettings appSettings= new AppSettings();
        public settingsPage()
        {
            InitializeComponent(); 
            AnimationContext = LayoutRoot;
            FillCinema();

            if (appSettings.LanguageSetting == "fr")
            {
                tgLanguage.IsChecked = true;
                tgLanguage.Content = "French";
            }
            else
            {
                tgLanguage.IsChecked = false;
                tgLanguage.Content = "Dutch";
            }
            longlistCinemas.ItemsSource = CollectionHelpers.CreateGroupedCinemas(new ObservableCollection<Cinema>(cinemas));
            txbCinema.DataContext = appSettings.CinemaSetting;

        }

        private void FillCinema()
        {
           cinemas =new ObservableCollection<Cinema>( (App.Current as App).bigCinemaList);
        }

       private void longlistCinemas_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            appSettings.CinemaSetting =(Cinema)longlistCinemas.SelectedItem;

            txbCinema.DataContext = appSettings.CinemaSetting;
            longlistCinemas.Visibility= Visibility.Collapsed;
            txbNotifyBack.Visibility = Visibility.Visible;
        }

        private void changeCinema_Click(object sender, RoutedEventArgs e)
        {
            longlistCinemas.Visibility= Visibility.Visible;
        }

        private void tgLanguage_Checked(object sender, RoutedEventArgs e)
        {
            tgLanguage.Content = "French";
            appSettings.LanguageSetting = "fr";
            txbNotifyBack.Visibility = Visibility.Visible;
        }

        private void tgLanguage_Unchecked(object sender, RoutedEventArgs e)
        {
            tgLanguage.Content = "Dutch";
            appSettings.LanguageSetting = "nl";
            txbNotifyBack.Visibility = Visibility.Visible;
        }      
    }
}