namespace LibraryManagementApp.Models
{
    public class LentBook
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public string Borrower { get; set; }
        public DateTime LendDate { get; set; }
        public Book Book { get; set; }
    }
}