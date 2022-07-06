namespace FullTextSearch.App.Services;

public static class Middlewares
{
    public static IApplicationBuilder FillRepository(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var generate = scope.ServiceProvider.GetRequiredService<BookGenerator>();
        var indexer = scope.ServiceProvider.GetRequiredService<ElasticService>();
        
        indexer.IndexBooks(generate.InCount(24));

        return app;
    }
}