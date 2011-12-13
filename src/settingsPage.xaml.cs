using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
            //listPickerCinemas.ItemsSource = cinemas;
            //listPickerCinemas.DisplayMemberPath = "CinemaName";

            //listPickerLanguage.Items.Add("French");
            //listPickerLanguage.Items.Add("Dutch");

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
            longlistCinemas.ItemsSource = CollectionHelpers.CreateGroupedOC(new ObservableCollection<Cinema>(cinemas));
            txbCinema.DataContext = appSettings.CinemaSetting;

        }

        private void FillCinema()
        {
            //Todo : parse thse from website
            cinemas =new ObservableCollection<Cinema>( (App.Current as App).bigCinemaList);
        }


        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
           


            //if (listPickerLanguage.SelectedItem.ToString() == "French")
            //    appSettings.LanguageSetting = "fr";
            //else
            //{
            //    appSettings.LanguageSetting = "nl";
            //}

            base.OnNavigatedFrom(e);
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            

            


            //string lang = "French";
            //if (appSettings.LanguageSetting != "fr")
            //    lang = "Dutch";

            //for (int i = 0; i < listPickerCinemas.Items.Count; i++)
            //{
            //    if (((string)listPickerLanguage.Items[i]) == lang)
            //    {
            //        listPickerLanguage.SelectedIndex = i;
            //        break;
            //    }
            //}

        }


        public class GroupedOC<T> : ObservableCollection<T>
        {
            /// <summary>
            /// The Group Title
            /// </summary>
            public string Title
            {
                get;
                set;
            }

            /// <summary>
            /// Constructor ensure that a Group Title is included
            /// </summary>
            /// <param name="name">string to be used as the Group Title</param>
            public GroupedOC(string name)
            {
                this.Title = name;
            }

            /// <summary>
            /// Returns true if the group has a count more than zero
            /// </summary>
            public bool HasItems
            {
                get
                {
                    return (Count != 0);
                }
                private set
                {
                }
            }
        }

        public static class CollectionHelpers
        {
            /// <summary>
            /// Groups a passed Contacts ObservableCollection
            /// </summary>
            /// <param name="InitialContactsList">Unordered collection of Contacts</param>
            /// <returns>Grouped Observable Collection of Contacts suitable for the LongListSelector</returns>
            public static ObservableCollection<GroupedOC<Cinema>> CreateGroupedOC(ObservableCollection<Cinema> InitialContactsList)
            {

                //Initialise the Grouped OC to populate and return
                ObservableCollection<GroupedOC<Cinema>> GroupedContacts = new ObservableCollection<GroupedOC<Cinema>>();

                //first sort our contacts collection into a temp List using LINQ
                var SortedList = (from con in InitialContactsList
                                  orderby con.CinemaName
                                  select con).ToList();

                //Now enumerate throw the alphabet creating empty groups objects
                //This ensure that the whole alphabet exists even if we never populate them
                string Alpha = "#abcdefghijklmnopqrstuvwxyz";
                foreach (char c in Alpha)
                {
                    //Create GroupedOC for given letter
                    GroupedOC<Cinema> thisGOC = new GroupedOC<Cinema>(c.ToString());

                    //Create a temp list with the appropriate Contacts that have this NameKey
                    var SubsetOfCons = (from con in SortedList
                                        where con.NameKey == c.ToString()
                                        select con).ToList<Cinema>();

                    //Populate the GroupedOC
                    foreach (Cinema csm in SubsetOfCons)
                    {
                        thisGOC.Add(csm);
                    }

                    //Add this GroupedOC to the observable collection that is being returned
                    // and the LongListSelector can be bound to.
                    GroupedContacts.Add(thisGOC);
                }
                return GroupedContacts;
            }
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