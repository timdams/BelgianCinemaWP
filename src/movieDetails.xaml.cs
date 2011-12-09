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
using Belgian_Cinema.Model;
using Microsoft.Phone.Controls;

namespace Belgian_Cinema
{
    public partial class movieDetails : PhoneApplicationPage
    {
        public Movie theMovie { get; set; }
        public movieDetails()
        {
            theMovie = (App.Current as App).selectedMovie;
            InitializeComponent();

            this.DataContext = theMovie;
        }
    }
}