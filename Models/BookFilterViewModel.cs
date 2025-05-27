namespace LibraryManagementApp.Models
{
    public class BookFilterViewModel
    {
        public string GenreFilter { get; set; }
        public string LastAppliedFilter { get; set; }
        public List<Book> Books { get; set; }
        public List<string> Genres { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; } = 3; 
    }
}