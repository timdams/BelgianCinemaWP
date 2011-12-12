using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
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
        public Movie()
        {
            Title = "UNKNOW ERROR";
            Duration = "N/A";
            Schedules = new ObservableCollection<Schedule>();
            ShortInfo = "N/A";
        }

        public string ShortInfo { get; set; }

        private string title;
        public string Title { get { return title; }  set { title = value.Replace("&#039;", "'").Trim(); }
        }
        
        
        public ObservableCollection<Schedule> Schedules { get; set; }

        public Visibility PlayingToday { 
            get
            {
                string now = string.Format("{0:00}", DateTime.Now.Day) + "/" + DateTime.Now.Month;
                var q = (from s in Schedules where s.Date == now select s).FirstOrDefault();
                if (q != null)
                    return Visibility.Visible;
                else return Visibility.Collapsed;
            }
        }
        
        public string Duration { get; set; }

        public string CoverUrl { get; set; }
        public string MoreInfo
        {
            get { return "http://www.cinebel.be" + moreInfo; }
            set { moreInfo = value; }
        }

        private string moreInfo;
      

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
