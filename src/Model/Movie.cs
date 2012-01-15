using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace Belgian_Cinema.Model
{
    public class Movie
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
                string now = string.Format("{0:00}", DateTime.Now.Day) + "/" + string.Format("{0:00}",DateTime.Now.Month);
                var q = (from s in Schedules where s.Date == now select s).FirstOrDefault();
                if (q != null)
                    return Visibility.Visible;
                return Visibility.Collapsed;
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
                res = schedule.ShowHours.Aggregate(res, (current, time) => current + (time + ",\t\t\n"));
            }
            return res;
        }   
    }
}
