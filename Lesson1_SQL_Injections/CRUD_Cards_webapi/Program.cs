using CRUD_Cards_webapi.Models;
using CRUD_Cards_webapi.Services;

using Microsoft.AspNetCore.Mvc;
using Thundire.Helpers;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IDebetCardsService, DebetCardsService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("debet", (IDebetCardsService debetCardsService) => debetCardsService.Get());

app.MapGet("debet/{id}", ([FromRoute] int id, IDebetCardsService debetCardsService) =>
{
    if (id <= 0)
    {
        return Results.BadRequest();
    }

    var result = debetCardsService.Get(id);

    if (result is not SuccessResult<DebetCardResponse> successResult)
    {
        return Results.NotFound();
    }

    return Results.Ok(successResult.CallBackData);
});

app.MapDelete("debet/{id}", ([FromRoute] int id, IDebetCardsService debetCardsService) =>
{
    if (id <= 0) return Results.BadRequest();
    debetCardsService.Delete(id);
    return Results.Ok();
});

app.MapPut("debet/{id}", ([FromRoute] int id, [FromBody] UpdateDebetCardRequest request, IDebetCardsService debetCardsService) =>
{
    if (id <= 0) return Results.BadRequest();

    // Validate

    var result = debetCardsService.Update(id, request);

    return !result.IsSuccess ? Results.NotFound() : Results.Ok();
});

app.MapPost("debet", ([FromBody] CreateDebetCardRequest request, IDebetCardsService debetCardsService) =>
{
    // Validate

    var result = debetCardsService.Create(request);

    if (result is SuccessResult<int> successResult)
    {
        return Results.Ok(successResult.CallBackData);
    }

    return Results.BadRequest();
});

app.Run();