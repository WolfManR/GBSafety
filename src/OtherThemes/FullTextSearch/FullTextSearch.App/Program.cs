using FullTextSearch.App.Models;
using FullTextSearch.App.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

builder.Services
    .AddSingleton<BookGenerator>();

var elasticConfiguration = builder.Configuration.GetSection("Elastic");

builder.Services
    .Configure<ElasticConfiguration>(elasticConfiguration)
    .AddScoped<ElasticService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.FillRepository();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
