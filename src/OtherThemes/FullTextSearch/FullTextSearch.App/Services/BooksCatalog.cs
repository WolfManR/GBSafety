using FullTextSearch.App.Models;

namespace FullTextSearch.App.Services;

public class BooksCatalog
{
    public static HashSet<Book> Books { get; set; }

    public void Add(Book book)
    {
        Books.Add(book);
    }

    public List<Book> List()
    {
        return Books.ToList();
    }
}