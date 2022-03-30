using BookingCatalog.Core;
using BookingCatalog.Graph_Neo4j_Data.Context;
using Neo4j.Driver;

namespace BookingCatalog.Graph_Neo4j_Data;

public class BooksRepository : IBooksRepository
{
    private readonly Neo4jContext _context;

    public BooksRepository(Neo4jContext context)
    {
        _context = context;
    }


    public int CountStoredBooks(string bookTitle)
    {
        return 0;
    }

    public async IAsyncEnumerable<Book> ListBooks(Author author)
    {
        await using var session = _context.AsyncSession();
        
        var result = await session.ReadTransactionAsync(GetAllBooks);

        foreach (var book in result)
        {
            yield return book;
        }
    }

    public async Task<bool> Store(Book book, Author author, int amount)
    {
        await using var session = _context.AsyncSession();

        try
        {
            var result1 = await session.WriteTransactionAsync(tx => AddBook(tx, book, author));
            var result2 = await session.ReadTransactionAsync(tx => CheckThatBookStored(tx, book));

            return true;
        }
        catch (Neo4jException ex)
        {
            return false;
        }
    }

    // create book with single author
    //merge (b:Book { title: 'Some story' , description: 'some described here', pages: 3 })
    //merge (a:Person:Author { firstname: 'Somebody', lastname: 'La Cruse' })
    //merge (a) <- [w:WRITE] - (b)

    // check that book stored
    //match(a:Book { title: 'Other story' })
    //with a
    //merge(s:Storage)
    //with a, s
    //optional match(s) - [st: Stored] - (a)
    //return st,a

    // add book to storage
    //match(a:Book { title: 'Other story' })
    //match(s:Storage)
    //create(s) <- [st: Stored {count:84}] - (a)

    // update Count of Stored books
    //match(s:Storage) <- [st: Stored] - (a:Book { title: 'Other story' })
    //set st += {count: 46}

    // get books
    //match (b:Book) return b

    private async Task<IResultSummary> AddBook(IAsyncTransaction transaction, Book book, Author author)
    {
        const string query = @"
merge (b:Book { title: '$title' , description: '$description', pages: $pages })
merge (a:Person:Author { firstName: '$firstName', lastName: '$lastName', age: $age })
merge (a) <- [w:WRITE] - (b)
";

        var cursor = await transaction.RunAsync(query, new
        {
            title = book.Title,
            description = book.Description,
            pages = book.Pages,

            firstName = author.FirstName,
            lastName = author.LastName,
            age = author.Age
        });

        return await cursor.ConsumeAsync();
    }
    private async Task<IResultSummary> CheckThatBookStored(IAsyncTransaction transaction, Book book)
    {
        const string query = @"
match (a:Book { title: '$title' , description: '$description', pages: $pages })
with a
merge (s:Storage)
with a, s
optional match(s) - [st: Stored] - (a)
return st,a
";

        var cursor = await transaction.RunAsync(query, new
        {
            title = book.Title,
            description = book.Description,
            pages = book.Pages
        });

        return await cursor.ConsumeAsync();
    }
    private async Task<IResultSummary> AddBookToStorage(IAsyncTransaction transaction, Book book, int amount)
    {
        const string query = @"
match (a:Book { title: '$title' , description: '$description', pages: $pages })
match(s:Storage)
create(s) <- [st: Stored {count: $amount}] - (a)
";

        var cursor = await transaction.RunAsync(query, new
        {
            title = book.Title,
            description = book.Description,
            pages = book.Pages,
            amount
        });

        return await cursor.ConsumeAsync();
    }
    private async Task<IResultSummary> UpdateCountOfStoredBooks(IAsyncTransaction transaction, Book book, int amount)
    {
        const string query = @"
match(s:Storage) <- [st: Stored] - (a:Book { title: '$title' , description: '$description', pages: $pages })
set st += {count: $amount}
";

        var cursor = await transaction.RunAsync(query, new
        {
            title = book.Title,
            description = book.Description,
            pages = book.Pages,
            amount
        });

        return await cursor.ConsumeAsync();
    }

    private async Task<IReadOnlyCollection<Book>> GetAllBooks(IAsyncTransaction transaction)
    {
        const string query = @"match (b:Book) return b";

        var cursor = await transaction.RunAsync(query);

        return await cursor.ToListAsync<Book>(record => new Book()
            {
                Title = record["title"].As<string>(),
                Description = record["description"].As<string>(),
                Pages = record["pages"].As<int>()
            });
    }
}