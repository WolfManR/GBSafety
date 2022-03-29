namespace BookingCatalog.Core;

public class Author : Person
{
    public ICollection<Book> WritedBooks { get; set; }
}