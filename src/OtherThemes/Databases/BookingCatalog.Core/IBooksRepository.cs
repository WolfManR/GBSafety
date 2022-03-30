namespace BookingCatalog.Core;

public interface IBooksRepository
{
    int CountStoredBooks(string bookTitle);
    IAsyncEnumerable<Book> ListBooks(Author author);
    Task<bool> Store(Book book, Author author, int count);
}