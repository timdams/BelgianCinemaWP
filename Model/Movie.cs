﻿using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Belgian_Cinema.Model
{
    public class Movie: INotifyPropertyChanged
    {
        public string Title { get; set; }
        public ObservableCollection<Schedule> Schedules { get; set; }
        public string Duration { get; set; }

      



        public string CoverUrl { get; set; }
        public string MoreInfo
        {
            get { return "http://www.cinebel.be" + moreInfo; }
            set { moreInfo = value; }
        }

        private string moreInfo;
        public Movie()
        {
            Title = "UNKNOW ERROR";
            Duration = "N/A";
            Schedules = new ObservableCollection<Schedule>();
           
        }

        public override string ToString()
        {
            string res = String.Format("{0} ({1}) \n\t", Title, Duration);
            foreach (var schedule in Schedules)
            {
                res += string.Format("{0},{1}\t{2} \n\t", schedule.Day, schedule.Date, schedule.AudioVersie);
                foreach (var time in schedule.ShowHours)
                {
                    res += time + ",\t\t\n";
                }
            }
            return res;
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
