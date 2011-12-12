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
using Microsoft.Phone.Tasks;

namespace Belgian_Cinema
{
    public partial class movieDetails : PhoneApplicationPage
    {
        public Movie theMovie { get; set; }
        private ShareLinkTask shareLinkTask;
        private EmailComposeTask emailComposeTask;
        public movieDetails()
        {
            theMovie = (App.Current as App).selectedMovie;
            InitializeComponent();

            this.DataContext = theMovie;

            shareLinkTask= new ShareLinkTask();
            emailComposeTask= new EmailComposeTask();
        }

        private void ShareLinkBtn_Click(object sender, EventArgs e)
        {
            shareLinkTask.Title = theMovie.Title;
            shareLinkTask.LinkUri = new Uri(theMovie.MoreInfo);
            shareLinkTask.Message = "Ready for the movies!";
            shareLinkTask.Show();
        }

        private void ShareEmailBtn_Click(object sender, EventArgs e)
        {
            AppSettings apps= new AppSettings();
            emailComposeTask.Subject = "I want to watch " + theMovie.Title;
            emailComposeTask.Body = "Hey they're playing " + theMovie.Title + " in " + apps.CinemaSetting.CinemaName +
                                    ".\n\r Moviehours are:\n\r";
            foreach (var s in theMovie.Schedules)
            {
                emailComposeTask.Body += s.ToString() + "\n";
            }

            emailComposeTask.Body += "\n More info here:" + theMovie.MoreInfo;
            emailComposeTask.Show();

        }

     
    }
}