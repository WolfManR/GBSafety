using CRUD_Cards_webapi.Dapper;
using CRUD_Cards_webapi.Dapper.Migrations;
using CRUD_Cards_webapi.EF;
using CRUD_Cards_webapi.Models;
using CRUD_Cards_webapi.Services;
using CRUD_Cards_webapi.Validations;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Thundire.Helpers;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<CardsDbContext>((p, o) => o.UseNpgsql(p.GetRequiredService<IConfiguration>().GetConnectionString("PostgreeEF")));

builder.Services
    .AddScoped<InitMigration>()
    .AddScoped<CardsDapperDbContext>(provider =>
    {
        var configuration = provider.GetRequiredService<IConfiguration>();
        var connectionString = configuration.GetConnectionString("PostgreeDapper");
        var database = configuration.GetValue<string>("DapperDatabase");
        return new CardsDapperDbContext(connectionString, database);
    })
    .AddScoped<DapperDebetCardsRepository>()
    .AddScoped<DapperDebetCardsService>();

builder.Services
    .AddScoped<EFCoreDebetCardsRepository>()
    .AddScoped<EFCoreDebetCardsService>();

builder.Services.AddFluentValidation();
builder.Services.AddScoped<IValidator<DebetCardBase>, DebetCardValidation>();

var app = builder.Build();

await using (var scope = app.Services.CreateAsyncScope())
{
    var dapperInitMigration = scope.ServiceProvider.GetRequiredService<InitMigration>();
    await dapperInitMigration.Up();

    await using var context = scope.ServiceProvider.GetRequiredService<CardsDbContext>();
    await context.Database.MigrateAsync();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("debet", static async (EFCoreDebetCardsService debetCardsService) =>
{
    var data = await debetCardsService.Get();
    return Results.Ok(data);
});

app.MapGet("debet/{id}", static async ([FromRoute] int id, EFCoreDebetCardsService debetCardsService) =>
{
    if (id <= 0)
    {
        return Results.BadRequest("id cannot be lower 1");
    }

    var result = await debetCardsService.Get(id);

    if (result is not SuccessResult<DebetCardResponse> successResult)
    {
        return Results.NotFound();
    }

    return Results.Ok(successResult.CallBackData);
});

app.MapDelete("debet/{id}", static async ([FromRoute] int id, EFCoreDebetCardsService debetCardsService) =>
{
    if (id <= 0) return Results.BadRequest("id cannot be lower 1");
    await debetCardsService.Delete(id);
    return Results.Ok();
});

app.MapPut("debet/{id}", static async ([FromRoute] int id, [FromBody] UpdateDebetCardRequest request, EFCoreDebetCardsService debetCardsService, IValidator<DebetCardBase> validation) =>
{
    if (id <= 0) return Results.BadRequest();

    var validationResult = await validation.ValidateAsync(request);
    if (!validationResult.IsValid) return Results.BadRequest(validationResult.Errors);

    var result = await debetCardsService.Update(id, request);

    return !result.IsSuccess ? Results.NotFound() : Results.Ok();
});

app.MapPost("debet", static async ([FromBody] CreateDebetCardRequest request, EFCoreDebetCardsService debetCardsService, IValidator<DebetCardBase> validation) =>
{
    var validationResult = await validation.ValidateAsync(request);
    if (!validationResult.IsValid) return Results.BadRequest(validationResult.Errors);

    var result = await debetCardsService.Create(request);

    if (result is SuccessResult<int> successResult)
    {
        return Results.Ok(successResult.CallBackData);
    }

    return Results.BadRequest();
});


app.MapGet("dapper/debet", static async (DapperDebetCardsService debetCardsService) =>
{
    var data = await debetCardsService.Get();
    return Results.Ok(data);
});

app.MapGet("dapper/debet/{id}", static async ([FromRoute] int id, DapperDebetCardsService debetCardsService) =>
{
    if (id <= 0)
    {
        return Results.BadRequest("id cannot be lower 1");
    }

    var result = await debetCardsService.Get(id);

    if (result is not SuccessResult<DebetCardResponse> successResult)
    {
        return Results.NotFound();
    }

    return Results.Ok(successResult.CallBackData);
});

app.MapDelete("dapper/debet/{id}", static async ([FromRoute] int id, DapperDebetCardsService debetCardsService) =>
{
    if (id <= 0) return Results.BadRequest("id cannot be lower 1");
    await debetCardsService.Delete(id);
    return Results.Ok();
});

app.MapPut("dapper/debet/{id}", static async ([FromRoute] int id, [FromBody] UpdateDebetCardRequest request, DapperDebetCardsService debetCardsService, IValidator<DebetCardBase> validation) =>
{
    if (id <= 0) return Results.BadRequest();

    var validationResult = await validation.ValidateAsync(request);
    if (!validationResult.IsValid) return Results.BadRequest(validationResult.Errors);

    var result = await debetCardsService.Update(id, request);

    return !result.IsSuccess ? Results.NotFound() : Results.Ok();
});

app.MapPost("dapper/debet", static async ([FromBody] CreateDebetCardRequest request, DapperDebetCardsService debetCardsService, IValidator<DebetCardBase> validation) =>
{
    var validationResult = await validation.ValidateAsync(request);
    if (!validationResult.IsValid) return Results.BadRequest(validationResult.Errors);

    var result = await debetCardsService.Create(request);

    if (result is SuccessResult<int> successResult)
    {
        return Results.Ok(successResult.CallBackData);
    }

    return Results.BadRequest();
});

app.Run();