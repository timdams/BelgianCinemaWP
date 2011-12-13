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
using HtmlAgilityPack;
using Microsoft.Phone.Net.NetworkInformation;
using WP7Contrib.View.Transitions.Animation;

namespace Belgian_Cinema
{
    public partial class MainPage : AnimatedBasePage
    {
        private AppSettings appSettings=new AppSettings();
        ObservableCollection<Movie>  movieList= new ObservableCollection<Movie>();
        
        // Constructor
        public MainPage()
        {
            InitializeComponent();
            lbresult.ItemsSource = movieList;
            AnimationContext = LayoutRoot;
      
        }

        private void UpdateMovieList()
        {
            try
            {


                if (NetworkInterface.GetIsNetworkAvailable())
                {

                    lbresult.Visibility = Visibility.Collapsed;
                    progressbar.Visibility = Visibility.Visible;
                    progressbarb.IsIndeterminate = true;
                    selectedCinema.DataContext = appSettings.CinemaSetting;
                    string downloadstring = "http://www.cinebel.be/" + appSettings.LanguageSetting;

                    downloadstring += "/bioscoop/" + ((Cinema) appSettings.CinemaSetting).CinemaId + "/";
                    HtmlWeb.LoadAsync(downloadstring,
                                      DownloadMainHtmlCompleted);
                }

                else
                {
                    MessageBox.Show("No active internet connection found.");
                    lbresult.Visibility = Visibility.Visible;
                    progressbar.Visibility = Visibility.Collapsed;
                    progressbarb.IsIndeterminate = false;
                }
            }
            catch(Exception e)
            {
                MessageBox.Show(
                    "An error occured. Probably because you don't have an active internet connection or the cinebel.be site is down");
            }
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            UpdateMovieList();
            base.OnNavigatedTo(e);
        }


        void  DownloadMainHtmlCompleted(object sender, HtmlDocumentLoadCompleted e)
        {
            try
            {


                HtmlDocument doc = e.Document;

                #region parse cinemas

                List<Cinema> cinemas = new List<Cinema>();
                var theathers = (from p in doc.DocumentNode.Descendants("select")
                                 where p.GetAttributeValue("id", "n/a") == "theaterSelector"
                                 select p).SingleOrDefault();
                if (theathers != null)
                {

                    foreach (var theather in theathers.Elements("option"))

                        if (theather.GetAttributeValue("value", "error") != "error")
                        {
                            cinemas.Add(new Cinema()
                                            {
                                                CinemaId = theather.GetAttributeValue("value", "0"),
                                                CinemaName = theather.InnerText
                                            });
                        }


                    int count = 0;
                    foreach (var cinema in theathers.Elements("#text"))
                    {

                        if (cinema.InnerText != null && count < cinemas.Count && cinema.InnerText.Trim() != "")
                        {
                            cinemas[count].CinemaName = cinema.InnerText.Trim();
                            count++;
                        }


                    }
                }
                (App.Current as App).bigCinemaList.Clear();
                (App.Current as App).bigCinemaList = cinemas;

                #endregion


                #region parse movie

                var movieNodes = from p in doc.DocumentNode.Descendants("div")
                                 where p.GetAttributeValue("class", "unknown") == "ssbd collapsable"
                                 select p;




                if (movieNodes != null)
                {
                    movieList.Clear();

                    foreach (HtmlNode movieNode in movieNodes)
                    {
                        Movie newMovie = new Movie();


                        //Movie title
                        var title =
                            (from p in movieNode.Descendants("span")
                             where p.GetAttributeValue("class", "unknown") == "contextualInfo"
                             select p).FirstOrDefault().InnerText.ToString().Trim();

                        if (title != null)
                        {
                            newMovie.Title = title;

                            //Rating (if any)
                            var rating = (from p in movieNode.Descendants("div")
                                          where p.GetAttributeValue("class", "unknown") == "ratingAverage"
                                          select p).FirstOrDefault();
                            if (rating != null)
                                ; //To do

                            //Movie cover
                            var covernode =
                                (from p in movieNode.Descendants("li")
                                 where p.GetAttributeValue("class", "n/a") == "moviePoster"
                                 select p).FirstOrDefault();

                            if (covernode != null)
                            {
                                newMovie.CoverUrl = covernode.Element("a").Element("img").GetAttributeValue("src", "n/a");
                            }

                            //Shortinfo
                            var infos =
                                (from p in movieNode.Descendants("div")
                                 where p.GetAttributeValue("class", "n/a") == "moviePersonList"
                                 select p).FirstOrDefault().Elements("p").FirstOrDefault().InnerText.Trim().Replace(
                                     "  ", "").Replace("\n", " ").Replace("&hellip;", "...");
                            newMovie.ShortInfo = infos;

                            //Duration
                            var dur = (from p in movieNode.Descendants("div")
                                       where p.GetAttributeValue("class", "o") == "scheduleMovieInfos"
                                       select p).FirstOrDefault().Element("ul").Elements("li");
                            foreach (HtmlNode htmlNode in dur)
                            {
                                if (htmlNode.InnerText.Length < 5)
                                {
                                    newMovie.Duration = htmlNode.InnerText;
                                    break;

                                }
                            }

                            //Movie url
                            newMovie.MoreInfo = (from p in movieNode.Descendants("div")
                                                 where
                                                     p.GetAttributeValue("class", "n/a") == "moviePersonList"
                                                 select p).SingleOrDefault().Element("a").GetAttributeValue("href",
                                                                                                            "n/a");

                            //Schedule
                            var scheduleNode = (from p in movieNode.Descendants("div")
                                                where p.GetAttributeValue("class", "o") == "schedulesItem"
                                                select p).SingleOrDefault().Element("table").Element("tbody").Elements(
                                                    "tr");
                            if (scheduleNode != null)
                            {

                                foreach (HtmlNode schednode in scheduleNode)
                                {
                                    Schedule newSched = new Schedule();
                                    //Dag
                                    var day = (from p in schednode.Element("th").Elements("span")
                                               where p.GetAttributeValue("class", "o") == "day"
                                               select p).SingleOrDefault();
                                    if (day != null)
                                        newSched.Day = day.InnerText.Trim();
                                    else
                                    {
                                        newSched.Day = newMovie.Schedules.Last().Day;
                                    }

                                    //Date
                                    newSched.Date = (from p in schednode.Element("th").Elements("span")
                                                     where p.GetAttributeValue("class", "o").Contains("date")
                                                     select p).SingleOrDefault().InnerText.Trim();

                                    //VideoVersie
                                    newSched.VideoVersie = (from p in schednode.Elements("td")
                                                            where
                                                                p.GetAttributeValue("class", "o").Contains(
                                                                    "VideoVersion")
                                                            select p.Element("abbr")).SingleOrDefault().InnerText.Trim();

                                    //Audio
                                    newSched.AudioVersie = (from p in schednode.Descendants("abbr")
                                                            where
                                                                p.GetAttributeValue("class", "N/A").Contains(
                                                                    "audioVersion")
                                                            select p).SingleOrDefault().InnerText.Trim();

                                    //Showhours
                                    var showhours = (from p in schednode.Descendants("div")
                                                     where p.GetAttributeValue("class", "o") == "hours"
                                                     select p).SingleOrDefault().Elements("span");
                                    foreach (var showhour in showhours)
                                    {
                                        newSched.ShowHours.Add(showhour.InnerText);
                                    }
                                    newMovie.Schedules.Add(newSched);
                                }
                            }
                            //Finaly, movie is parsed
                            movieList.Add(newMovie);
                        }
                    }
                }
                if (movieList.Count() == 0)
                {
                    MessageBox.Show("This theater is not showing any movies this week.");
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show("Error occured while fetching results. (Full error:" + ex.Message);
            }

            #endregion  

            lbresult.Visibility = Visibility.Visible;
            progressbar.Visibility = Visibility.Collapsed;
            progressbarb.IsIndeterminate = false;
        }

        private void lbresult_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

       //     (App.Current as App).selectedMovie = (Movie) lbresult.SelectedItem;

            NavigationService.Navigate(new Uri("/movieDetails.xaml", UriKind.RelativeOrAbsolute));
        }

        private void ApplicationBarIconSettings_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/settingsPage.xaml", UriKind.RelativeOrAbsolute));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            (App.Current as App).selectedMovie = (Movie) ((Image) sender).DataContext;
            NavigationService.Navigate(new Uri("/movieDetails.xaml", UriKind.RelativeOrAbsolute));
        }

        private void ApplicationBarMenuItem_Click(object sender, EventArgs e)
        {
            //NavigationService.Navigate(new Uri("/AboutPage.xaml", UriKind.RelativeOrAbsolute));
            NavigationService.Navigate(new Uri("/YourLastAboutDialog;component/AboutPage.xaml", UriKind.Relative));
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            UpdateMovieList();
        }

      
    }
}