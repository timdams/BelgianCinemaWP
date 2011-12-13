using System.Collections.ObjectModel;
using System.Linq;

namespace Belgian_Cinema.Model
{
    public class Schedule
    {
        public string Day { get; set; }
        public string Date { get; set; }
        public string AudioVersie { get; set; }
        public string VideoVersie { get; set; }
        public string AudioVersieColor { 
            get
            {
                switch (AudioVersie)
                {
                    case "DV":
                        return "red";
                    case "OV":
                        return "orange";
                    case "NV":
                        return "blue";
                    case "FV":
                        return "green";
                    default:
                        return "teal";
                }
            } 
        }
        public string VideoVersieColor 
        { 
            get
            {
                switch (VideoVersie)
                {
                    case "3D": return "DarkBlue";
                    case "Dig":return "Gray";
                    default: return "Gray";
                }
            }
        }
        public ObservableCollection<string> ShowHours { get; set; }

        public Schedule()
        {
            ShowHours = new ObservableCollection<string>();
            Day = "N/A";
            Date = "N/A";
            AudioVersie = "N/A";
            VideoVersie = "N/A";
        }

        public override string ToString()
        {
            string s = Day + " " + Date + ": ";

            return this.ShowHours.Aggregate(s, (current, hours) => current + (hours + "  "));
        }
    }
}
