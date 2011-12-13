using System;
using Belgian_Cinema.Model;
using Belgian_Cinema.UtilityClasses;
using Microsoft.Phone.Tasks;
using WP7Contrib.View.Transitions.Animation;

namespace Belgian_Cinema
{
    public partial class MovieDetails : AnimatedBasePage
    {
        public Movie theMovie { get; set; }
        private ShareLinkTask shareLinkTask;
        private EmailComposeTask emailComposeTask;

        public MovieDetails()
        {
            InitializeComponent();

            theMovie = (App.Current as App).selectedMovie;
            this.DataContext = theMovie;

            shareLinkTask= new ShareLinkTask();
            emailComposeTask= new EmailComposeTask();

            AnimationContext = LayoutRoot;
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
            var apps= new AppSettings();

            emailComposeTask.Subject = "I want to watch " + theMovie.Title;
            emailComposeTask.Body = "Hey they're playing " + theMovie.Title + " in " + apps.CinemaSetting.CinemaName +
                                    ".\n\r Moviehours are:\n\r";
            foreach (var s in theMovie.Schedules)
            {
                emailComposeTask.Body += s + "\n";
            }

            emailComposeTask.Body += "\n More info here:" + theMovie.MoreInfo;
            emailComposeTask.Show();
        }    
    }
}