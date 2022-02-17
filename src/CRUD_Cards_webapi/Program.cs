using CRUD_Cards_webapi.EF;
using CRUD_Cards_webapi.Models;
using CRUD_Cards_webapi.Services;
using CRUD_Cards_webapi.Validations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Thundire.Helpers;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<CardsDbContext>((p, o) => o.UseNpgsql(p.GetRequiredService<IConfiguration>().GetConnectionString("Postgree")));

builder.Services.AddScoped<IDebetCardsService, DebetCardsService>();

var app = builder.Build();

await using (var scope = app.Services.CreateAsyncScope())
{
    await using var context = scope.ServiceProvider.GetRequiredService<CardsDbContext>();
    await context.Database.MigrateAsync();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("debet", static async (IDebetCardsService debetCardsService) =>
{
    var data = await debetCardsService.Get();
    return Results.Ok(data);
});

app.MapGet("debet/{id}", static async ([FromRoute] int id, IDebetCardsService debetCardsService) =>
{
    if (id <= 0)
    {
        return Results.BadRequest();
    }

    var result = await debetCardsService.Get(id);

    if (result is not SuccessResult<DebetCardResponse> successResult)
    {
        return Results.NotFound();
    }

    return Results.Ok(successResult.CallBackData);
});

app.MapDelete("debet/{id}", static async ([FromRoute] int id, IDebetCardsService debetCardsService) =>
{
    if (id <= 0) return Results.BadRequest();
    await debetCardsService.Delete(id);
    return Results.Ok();
});

app.MapPut("debet/{id}", static async ([FromRoute] int id, [FromBody] UpdateDebetCardRequest request, IDebetCardsService debetCardsService, DebetCardValidation validation) =>
{
    if (id <= 0) return Results.BadRequest();

    var validationResult = await validation.ValidateAsync(request);
    if (!validationResult.IsValid) return Results.BadRequest();

    var result = await debetCardsService.Update(id, request);

    return !result.IsSuccess ? Results.NotFound() : Results.Ok();
});

app.MapPost("debet", static async ([FromBody] CreateDebetCardRequest request, IDebetCardsService debetCardsService, DebetCardValidation validation) =>
{
    var validationResult = await validation.ValidateAsync(request);
    if (!validationResult.IsValid) return Results.BadRequest();

    var result = await debetCardsService.Create(request);

    if (result is SuccessResult<int> successResult)
    {
        return Results.Ok(successResult.CallBackData);
    }

    return Results.BadRequest();
});

app.Run();