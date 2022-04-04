using FullTextSearch.App.Models;
using Microsoft.Extensions.Options;
using Nest;

namespace FullTextSearch.App.Services;

public class ElasticService
{
    private readonly ElasticClient _client;

    public ElasticService(IOptions<ElasticConfiguration> configuration)
    {
        var configuration1 = configuration.Value;
        _client = new ElasticClient(BuildConnectionSetting(configuration1));
        SetIndexes(configuration1);
    }

    public void IndexBooks(IReadOnlyCollection<Book> books)
    {
        var result = _client.BulkAll(books, d => d
            .BackOffRetries(2)
            .BackOffTime("30s")
            .RefreshOnCompleted()
            .MaxDegreeOfParallelism(4)
            .Size(100));
        result.Wait(TimeSpan.FromSeconds(2000), response =>
        {
            var items = response.Items;
            Console.WriteLine(string.Join("\r\n", items));
        });
    }

    public void IndexBook(Book book)
    {
        var result = _client.IndexDocument(book);
    }

    private void SetIndexes(ElasticConfiguration configuration)
    {
        _client.Indices.Create(configuration.Index, createDescriptor => createDescriptor.Map<Book>(map => map
            .AutoMap()
            .Properties(d => d
                .Nested<Author>(n => n.Name(b => b.Author.FullName).AutoMap())
            )));
    }

    private static IConnectionSettingsValues BuildConnectionSetting(ElasticConfiguration configuration)
    {
        Uri uri = new (configuration.Uri);
        return new ConnectionSettings(uri)
            .DefaultMappingFor<Book>(m => m.IndexName(configuration.Index));
    }
}