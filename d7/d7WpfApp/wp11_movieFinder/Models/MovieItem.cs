namespace wp11_movieFinder.Models
{

    public class MovieItem
    {
        // readonly => auto property로 만듦
        public string Title { get; set; }
        public string Original_Title { get; set; }
        public bool Adult { get; set; }
        public string Poster_Path { get; set; }
        public double Vote_Average { get; set; }
        public double Popularity { get; set; }
        public string Release_date { get; set; }
        public string Original_Language { get; set; }
        public int Id { get; set; }     // key
        public string OverView { get; set; }
        
    }
}
