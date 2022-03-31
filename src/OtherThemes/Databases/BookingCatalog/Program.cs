using BookingCatalog.Core;
using BookingCatalog.Graph_Neo4j_Data;
using BookingCatalog.Graph_Neo4j_Data.Context;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services
    .AddSingleton<Neo4jOptions>(provider =>
    {
        var configuration = provider.GetRequiredService<IConfiguration>();

        return new Neo4jOptions()
        {
            Uri = configuration.GetValue<string>("ConnectionStrings:Neo4j:Uri"),
            Login = configuration.GetValue<string>("ConnectionStrings:Neo4j:Login"),
            Password = configuration.GetValue<string>("ConnectionStrings:Neo4j:Password"),
        };
    })
    .AddScoped<Neo4jContext>()
    .AddScoped<IBooksRepository, BooksRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapPost("books/store", StoreBooks);

app.Run();


async Task<IResult> StoreBooks(StoreBooksRequest request, IBooksRepository booksRepository)
{
   await booksRepository.Store(request.Book, request.Author, request.Amount);
   return Results.Ok();
}

record StoreBooksRequest(Book Book, Author Author, int Amount);