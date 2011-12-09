using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

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
                        break;
                    case "OV":
                        return "orange";
                        break;
                    case "NV":
                        return "blue";
                        break;
                    case "FV":
                        return "green";
                        break;
                        
                    default:
                        return "teal";
                        break;
                        
                }
            } 
        }
        public string VideoVersieColor { get
        {
            switch (VideoVersie)
            {
                case "3D":
                    return "DarkBlue";
                    break;

                case "Dig":
                    return "Gray";
                    break;
                default:
                    return "Gray";

            }
        }}
        public ObservableCollection<string> ShowHours { get; set; }

        public Schedule()
        {
            ShowHours = new ObservableCollection<string>();
            Day = "N/A";
            Date = "N/A";
            AudioVersie = "N/A";
            VideoVersie = "N/A";
        }
    }
}
