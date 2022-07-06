using Bogus;
using FullTextSearch.App.Models;

namespace FullTextSearch.App.Services;

public class BookGenerator
{
    private static Faker<Book> _generator = new Faker<Book>().Rules((f, b) =>
    {
        b.Id = Guid.NewGuid().ToString();
        b.Title = f.Commerce.ProductName();
        b.Author = new Author()
        {
            FirstName = f.Name.FirstName(),
            LastName = f.Name.LastName()
        };
    });

    public IReadOnlyList<Book> InCount(int count = 1)
    {
        return _generator.Generate(count);
    }
}