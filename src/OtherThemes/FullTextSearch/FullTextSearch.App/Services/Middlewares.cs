namespace FullTextSearch.App.Services;

public static class Middlewares
{
    public static IApplicationBuilder FillRepository(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var generate = scope.ServiceProvider.GetRequiredService<BookGenerator>();
        var catalog = scope.ServiceProvider.GetRequiredService<BooksCatalog>();

        foreach (var book in generate.InCount(24))
        {
            catalog.Add(book);
        }
        return app;
    }
}