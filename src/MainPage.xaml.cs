﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Belgian_Cinema.Model;
using Belgian_Cinema.UtilityClasses;
using HtmlAgilityPack;

using NetworkInterface = System.Net.NetworkInformation.NetworkInterface;

namespace Belgian_Cinema
{
    public partial class MainPage
    {
        private AppSettings appSettings = new AppSettings();
        ObservableCollection<Movie>  movieList = new ObservableCollection<Movie>();
        
        // Constructor
        public MainPage()
        {
            InitializeComponent();
            lbresult.ItemsSource = movieList;
            AnimationContext = LayoutRoot;
            selectedCinema.DataContext = appSettings.CinemaSetting;
  
        }

        private void UpdateMovieList()
        {
           
            if (appSettings.LatestDownloadedMovieListCache.cinemasource.CinemaId != appSettings.CinemaSetting.CinemaId ||
                DateTime.Now.Date.Subtract(appSettings.LatestDownloadedMovieListCache.LastDownloadedTime) > new TimeSpan(23, 59, 59)
                || appSettings.LanguageSetting != appSettings.LatestDownloadedMovieListCache.Language)
            {

                DownloadHtmlMovieData();
            }
            else
            {
                movieList = new ObservableCollection<Movie>(appSettings.LatestDownloadedMovieListCache.MovieList);
            }

        }

        private void DownloadHtmlMovieData()
        {
            try
            {
                if (NetworkInterface.GetIsNetworkAvailable())
                {
                    lbresult.Visibility = Visibility.Collapsed;
                    progressbar.Visibility = Visibility.Visible;
                    progressbarb.IsIndeterminate = true;
                    
                    string downloadstring = "http://www.cinebel.be/" + appSettings.LanguageSetting;

                    downloadstring += "/bioscoop/" + appSettings.CinemaSetting.CinemaId + "/";
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
            catch
            {
                MessageBox.Show(
                    "An error occured. Probably because you don't have an active internet connection or the cinebel.be site is down");
            }
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            //if((App.Current as App).NeedUpdate)
                UpdateMovieList();
                lbresult.ItemsSource = movieList;
                AnimationContext = LayoutRoot;
                selectedCinema.DataContext = appSettings.CinemaSetting;
            base.OnNavigatedTo(e);
        }

        void  DownloadMainHtmlCompleted(object sender, HtmlDocumentLoadCompleted e)
        {
            try
            {
                HtmlDocument doc = e.Document;
              
                ParseHtmlFile(doc);

                
            }
            catch (Exception ex)
            {

                MessageBox.Show("Error occured while fetching results. (Full error:" + ex.Message);
            }
            finally
            {
                lbresult.Visibility = Visibility.Visible;
                progressbar.Visibility = Visibility.Collapsed;
                progressbarb.IsIndeterminate = false;
            }

        }

        private void ParseHtmlFile(HtmlDocument doc)
        {
            #region parse cinemas

            if (doc != null)
            {
                var cinemas = new List<Cinema>();
                var theathers = (from p in doc.DocumentNode.Descendants("select")
                                 where p.GetAttributeValue("id", "n/a") == "theaterSelector"
                                 select p).SingleOrDefault();
                if (theathers != null)
                {
                    cinemas.AddRange(from theather in theathers.Elements("option")
                                     where theather.GetAttributeValue("value", "error") != "error"
                                     select new Cinema
                                                {
                                                    CinemaId = theather.GetAttributeValue("value", "0"),
                                                    CinemaName = theather.InnerText
                                                });


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
                appSettings.CinemaListSetting.Clear();
                appSettings.CinemaListSetting = cinemas;
               

                #endregion

                #region parse movie

                var movieNodes = from p in doc.DocumentNode.Descendants("div")
                                 where p.GetAttributeValue("class", "unknown") == "ssbd collapsable"
                                 select p;

                movieList.Clear();
                foreach (HtmlNode movieNode in movieNodes)
                {
                    ExtractMovieData(movieNode);
                }

                
                //Succes

                if (movieList.Count() == 0)
                {
                    MessageBox.Show("This theater is not showing any movies this week.");
                }
                else
                {

                    //Save movie
                    var textw = new StringWriter();
                    doc.Save(textw);

                    appSettings.LatestDownloadedMovieListCache = new DownloadedMovieListCache()
                    {
                        cinemasource = appSettings.CinemaSetting,
                        LastDownloadedTime = DateTime.Now,
                        MovieList = movieList,
                        Language = appSettings.LanguageSetting
                    };
                }
            }
            else
                MessageBox.Show("Cinebel site appears to be down. Please retry some other time");

            #endregion
        }

        private void ExtractMovieData(HtmlNode movieNode)
        {
            var newMovie = new Movie();


            //Movie title
            var firstOrDefault = (from p in movieNode.Descendants("span")
                                  where p.GetAttributeValue("class", "unknown") == "contextualInfo"
                                  select p).FirstOrDefault();
            if (firstOrDefault != null)
            {
                var title =
                    firstOrDefault.InnerText.Trim();

                newMovie.Title = title;
            }

            //Movie cover
            var covernode =
                (from p in movieNode.Descendants("li")
                 where p.GetAttributeValue("class", "n/a") == "moviePoster"
                 select p).FirstOrDefault();

            if (covernode != null)
            {
                newMovie.CoverUrl =
                    (covernode.Element("a").Element("img").GetAttributeValue("src", "n/a")).Replace("/small/",
                                                                                                    "/medium/");
            }

            //Shortinfo
            var orDefault = (from p in movieNode.Descendants("div")
                             where p.GetAttributeValue("class", "n/a") == "moviePersonList"
                             select p).FirstOrDefault();
            if (orDefault != null)
            {
                HtmlNode htmlNode = orDefault.Elements("p").FirstOrDefault();
                if (htmlNode != null)
                {
                    var infos =
                        htmlNode.InnerText.Trim().Replace(
                            "  ", "").Replace("\n", " ").Replace("&hellip;", "...");
                    newMovie.ShortInfo = infos;
                }
            }

            //Duration
            var duration = (from p in movieNode.Descendants("div")
                            where p.GetAttributeValue("class", "o") == "scheduleMovieInfos"
                            select p).FirstOrDefault();
            if (duration != null)
            {
                var dur = duration.Element("ul").Elements("li");
                foreach (HtmlNode htmlNode in dur)
                {
                    if (htmlNode.InnerText.Length < 5)
                    {
                        newMovie.Duration = htmlNode.InnerText;
                        break;
                    }
                }
            }

            //Movie url
            var movieURL = (from p in movieNode.Descendants("div")
                            where
                                p.GetAttributeValue("class", "n/a") == "moviePersonList"
                            select p).SingleOrDefault();
            if (movieURL != null)
                newMovie.MoreInfo = "http://www.cinebel.be" + movieURL.Element("a").GetAttributeValue("href",
                                                                            "n/a");

            //Schedule
            var schedule = (from p in movieNode.Descendants("div")
                            where p.GetAttributeValue("class", "o") == "schedulesItem"
                            select p).SingleOrDefault();
            if (schedule != null)
            {
                var scheduleNode = schedule.Element("table").Element("tbody").Elements(
                    "tr");
                if (scheduleNode != null)
                {
                    ExtractSheduleInfo(scheduleNode, newMovie);
                }
            }
            //Finaly, movie is parsed
            movieList.Add(newMovie);
        }

        private static void ExtractSheduleInfo(IEnumerable<HtmlNode> scheduleNode, Movie newMovie)
        {
            foreach (HtmlNode schednode in scheduleNode)
            {
                var newSched = new Schedule();
                //Dag
                var day = (from p in schednode.Element("th").Elements("span")
                           where p.GetAttributeValue("class", "o") == "day"
                           select p).SingleOrDefault();
                newSched.Day = day != null ? day.InnerText.Trim() : newMovie.Schedules.Last().Day;

                //Date
                var schedDate = (from p in schednode.Element("th").Elements("span")
                                 where p.GetAttributeValue("class", "o").Contains("date")
                                 select p).SingleOrDefault();
                if (schedDate != null)
                    newSched.Date = schedDate.InnerText.Trim();

                //VideoVersie
                var vidvers = (from p in schednode.Elements("td")
                                 where
                                     p.GetAttributeValue("class", "o").Contains(
                                         "VideoVersion")
                                 select p.Element("abbr")).SingleOrDefault();
                if (vidvers != null)
                    newSched.VideoVersie = vidvers.InnerText.Trim();

                //Audio
                HtmlNode audiover = (from p in schednode.Descendants("abbr")
                                 where
                                     p.GetAttributeValue("class", "N/A").Contains(
                                         "audioVersion")
                                 select p).SingleOrDefault();
                if (audiover != null)
                    newSched.AudioVersie = audiover.InnerText.Trim();

                //Showhours
                var singleOrDefault = (from p in schednode.Descendants("div")
                                       where p.GetAttributeValue("class", "o") == "hours"
                                       select p).SingleOrDefault();
                if (singleOrDefault != null)
                {
                    var showhours = singleOrDefault.Elements("span");
                    foreach (var showhour in showhours)
                    {
                        newSched.ShowHours.Add(showhour.InnerText);
                    }
                }
                newMovie.Schedules.Add(newSched);
            }
        }

        private void ApplicationBarIconSettings_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/settingsPage.xaml", UriKind.RelativeOrAbsolute));
        }

        private void MovieImageClick(object sender, RoutedEventArgs e)
        {
            (App.Current as App).selectedMovie = (Movie) ((Image) sender).DataContext;
            NavigationService.Navigate(new Uri("/MovieDetails.xaml", UriKind.RelativeOrAbsolute));
        }

        private void ApplicationBarMenuItem_Click(object sender, EventArgs e)
        {
           NavigationService.Navigate(new Uri("/YourLastAboutDialog;component/AboutPage.xaml", UriKind.Relative));
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            DownloadHtmlMovieData();
        }

    }
}