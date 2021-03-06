using App.Metrics;
using App.Metrics.AspNetCore;
using App.Metrics.Formatters.Prometheus;
using CRUD_Cards_webapi.Dapper;
using CRUD_Cards_webapi.EF;
using CRUD_Cards_webapi.Models;
using CRUD_Cards_webapi.Services;
using CRUD_Cards_webapi.Validations;

using FluentMigrator.Runner;

using FluentValidation;
using FluentValidation.AspNetCore;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Thundire.Helpers;

const string EFCoreApiGroup = "EF";
const string DapperApiGroup = "DAPPER";

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<CardsDbContext>((p, o) =>
{
    var configuration = p.GetRequiredService<IConfiguration>();
    var connectionString = configuration.GetConnectionStringBuilder("EF").BuildWithDatabase();
    o.UseNpgsql(connectionString);
});

builder.Services
    .AddScoped<CardsDapperDbContext>(provider =>
    {
        var configuration = provider.GetRequiredService<IConfiguration>();
        var connectionString = configuration.GetConnectionStringBuilder("Dapper");
        return new CardsDapperDbContext(connectionString.BuildWithoutDatabase(), connectionString.Database);
    })
    .AddScoped<DapperDebetCardsRepository>()
    .AddScoped<DapperDebetCardsService>();

builder.Services
    .AddFluentMigratorCore()
    .ConfigureRunner(c => c
        .AddPostgres()
        .WithGlobalConnectionString(provider =>
        {
            var configuration = provider.GetRequiredService<IConfiguration>();
            var connectionString = configuration.GetConnectionStringBuilder("Dapper").BuildWithDatabase();
            return connectionString;
        })
        .ScanIn(typeof(Program).Assembly).For.Migrations());

builder.Services
    .AddScoped<EFCoreDebetCardsRepository>()
    .AddScoped<EFCoreDebetCardsService>();

builder.Services.AddFluentValidation();
builder.Services.AddScoped<IValidator<DebetCardBase>, DebetCardValidation>();

builder.Services.AddMetrics(metricsBuilder =>
{
    metricsBuilder
        .OutputMetrics.AsPrometheusPlainText()
        .OutputMetrics.AsPrometheusProtobuf();
});

builder.Host
    .UseMetricsWebTracking()
    .UseMetrics(o => o.EndpointOptions = mo =>
    {
        mo.MetricsTextEndpointOutputFormatter = new MetricsPrometheusTextOutputFormatter();
        mo.MetricsEndpointOutputFormatter = new MetricsPrometheusProtobufOutputFormatter();
        mo.EnvironmentInfoEndpointEnabled = false;
    });

var app = builder.Build();

await using (var scope = app.Services.CreateAsyncScope())
{
    try
    {
        await using var context = scope.ServiceProvider.GetRequiredService<CardsDbContext>();
        await context.Database.MigrateAsync();

        await using (var dapperContext = scope.ServiceProvider.GetRequiredService<CardsDapperDbContext>())
        {
            await dapperContext.DisposeAsync();
        }

        var dapperMigration = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
        dapperMigration.MigrateUp();
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogCritical(ex, "Fail connect to databases");
        Environment.Exit(-1);
        return;
    }
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
}).WithTags(EFCoreApiGroup);

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
}).WithTags(EFCoreApiGroup);

app.MapDelete("debet/{id}", static async ([FromRoute] int id, EFCoreDebetCardsService debetCardsService) =>
{
    if (id <= 0) return Results.BadRequest("id cannot be lower 1");
    await debetCardsService.Delete(id);
    return Results.Ok();
}).WithTags(EFCoreApiGroup);

app.MapPut("debet/{id}", static async ([FromRoute] int id, [FromBody] UpdateDebetCardRequest request, EFCoreDebetCardsService debetCardsService, IValidator<DebetCardBase> validation) =>
{
    if (id <= 0) return Results.BadRequest();

    var validationResult = await validation.ValidateAsync(request);
    if (!validationResult.IsValid) return Results.BadRequest(validationResult.Errors);

    var result = await debetCardsService.Update(id, request);

    return !result.IsSuccess ? Results.NotFound() : Results.Ok();
}).WithTags(EFCoreApiGroup);

app.MapPost("debet", static async (
    [FromBody] CreateDebetCardRequest request,
    EFCoreDebetCardsService debetCardsService,
    IValidator<DebetCardBase> validation,
    IMetrics metrics) =>
{
    var validationResult = await validation.ValidateAsync(request);
    if (!validationResult.IsValid) return Results.BadRequest(validationResult.Errors);

    var result = await debetCardsService.Create(request);

    if (result is SuccessResult<int> successResult)
    {
        metrics.Measure.Counter.Increment(DapperMetricsRegistry.CreatedDebetCardsCounter);
        return Results.Ok(successResult.CallBackData);
    }

    return Results.BadRequest();
}).WithTags(EFCoreApiGroup);


app.MapGet("dapper/debet", static async (DapperDebetCardsService debetCardsService) =>
{
    var data = await debetCardsService.Get();
    return Results.Ok(data);
}).WithTags(DapperApiGroup);

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
}).WithTags(DapperApiGroup);

app.MapDelete("dapper/debet/{id}", static async ([FromRoute] int id, DapperDebetCardsService debetCardsService) =>
{
    if (id <= 0) return Results.BadRequest("id cannot be lower 1");
    await debetCardsService.Delete(id);
    return Results.Ok();
}).WithTags(DapperApiGroup);

app.MapPut("dapper/debet/{id}", static async ([FromRoute] int id, [FromBody] UpdateDebetCardRequest request, DapperDebetCardsService debetCardsService, IValidator<DebetCardBase> validation) =>
{
    if (id <= 0) return Results.BadRequest();

    var validationResult = await validation.ValidateAsync(request);
    if (!validationResult.IsValid) return Results.BadRequest(validationResult.Errors);

    var result = await debetCardsService.Update(id, request);

    return !result.IsSuccess ? Results.NotFound() : Results.Ok();
}).WithTags(DapperApiGroup);

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
}).WithTags(DapperApiGroup);

app.Run();