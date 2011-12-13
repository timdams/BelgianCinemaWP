using System.Collections.ObjectModel;
using System.Linq;
using Belgian_Cinema.Model;

namespace Belgian_Cinema.UtilityClasses
{
    public class GroupedCinemas<T> : ObservableCollection<T>
    {
        /// <summary>
        /// The Group Title
        /// </summary>
        public string Title
        {
            get;
            set;
        }

        /// <summary>
        /// Constructor ensure that a Group Title is included
        /// </summary>
        /// <param name="name">string to be used as the Group Title</param>
        public GroupedCinemas(string name)
        {
            this.Title = name;
        }

        /// <summary>
        /// Returns true if the group has a count more than zero
        /// </summary>
        public bool HasItems
        {
            get
            {
                return (Count != 0);
            }
        }
    }

    public static class CollectionHelpers
    {
        /// <summary>
        /// Groups a passed Contacts ObservableCollection
        /// </summary>
        /// <param name="InitialCinemasList">Unordered collection of Contacts</param>
        /// <returns>Grouped Observable Collection of Contacts suitable for the LongListSelector</returns>
        public static ObservableCollection<GroupedCinemas<Cinema>> CreateGroupedCinemas(ObservableCollection<Cinema> InitialCinemasList)
        {

            //Initialise the Grouped OC to populate and return
            var groupedCinemas = new ObservableCollection<GroupedCinemas<Cinema>>();

            //first sort our contacts collection into a temp List using LINQ
            var SortedList = (from con in InitialCinemasList
                              orderby con.CinemaName
                              select con).ToList();

            //Now enumerate throw the alphabet creating empty groups objects
            //This ensure that the whole alphabet exists even if we never populate them
            const string Alpha = "#abcdefghijklmnopqrstuvwxyz";
            foreach (char c in Alpha)
            {
                //Create GroupedCinemas for given letter
                var thisGOC = new GroupedCinemas<Cinema>(c.ToString());

                //Create a temp list with the appropriate Contacts that have this NameKey
                var subsetOfCinemas = (from con in SortedList
                                       where con.NameKey == c.ToString()
                                       select con).ToList<Cinema>();

                //Populate the GroupedCinemas
                foreach (Cinema csm in subsetOfCinemas)
                {
                    thisGOC.Add(csm);
                }

                //Add this GroupedCinemas to the observable collection that is being returned
                // and the LongListSelector can be bound to.
                groupedCinemas.Add(thisGOC);
            }
            return groupedCinemas;
        }
    }
}
