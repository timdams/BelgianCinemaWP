namespace Belgian_Cinema.Model
{
    public class Cinema
    {
      

        public Cinema(string name, string id)
        {
            this.CinemaId = id;
            this.CinemaName = name;
        }
        public Cinema():this("n/a","0")
        {
  
        }
        public string CinemaName { get; set; }
        public string CinemaId { get; set; }

        public override string ToString()
        {

            return CinemaName;
        }

        private string _NameKey;
        public string NameKey
        {
            get
            {
                //If not set then
                if (_NameKey == null)
                {
                    //get the first letter of the Name
                    char key = char.ToLower(this.CinemaName[0]);
                    if (key < 'a' || key > 'z')
                    {
                        key = '#';
                    }
                    _NameKey = key.ToString();
                }
                return _NameKey;
            }
        }
    }
}
