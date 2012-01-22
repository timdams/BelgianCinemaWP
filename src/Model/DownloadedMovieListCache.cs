using System;
using System.Collections.ObjectModel;

namespace Belgian_Cinema.Model
{
    public class DownloadedMovieListCache
    {
        public Cinema cinemasource { get; set; }
        public ObservableCollection<Movie> MovieList { get; set; }
        public DateTime LastDownloadedTime { get; set; }
        public string Language { get; set; }
    }
}
