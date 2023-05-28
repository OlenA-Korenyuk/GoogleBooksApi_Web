namespace GoogleBooksApi_Web
{
    public class Book
    {
        public string Title { get; set; }
        public string[] Authors { get; set; }
        public int PageCount { get; set; }
        public string Publisher { get; set; }
        public string Description { get; set; }
    }
    
    public class UserGenres
    {
        public List<string> FavoriteGenres { get; set; }
        public List<string> UnwantedGenres { get; set; }

        public UserGenres()
        {
            FavoriteGenres = new List<string>();
            UnwantedGenres = new List<string>();
        }
    }
}
