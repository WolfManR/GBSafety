using Authentication.Domain;
using Identity_Server;
using Identity_Server.DAL;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

const string corsPolicyAlias = "AuthPolicy";

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services
    .AddDbContext<IdentityDbContext>((provider, options) =>
    {
        var connectionString = provider.GetRequiredService<IConfiguration>().GetConnectionString("Identity");
        options.UseNpgsql(connectionString);
    })
    .AddIdentity<AuthenticationUser, IdentityRole>(options =>
    {
        options.Password.RequiredLength = 4;
        options.Password.RequireDigit = false;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = false;
    })
    .AddEntityFrameworkStores<IdentityDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddScoped<AuthenticationService>();

builder.Services.RegisterBaseCors(corsPolicyAlias);

builder.Services.ConfigureAuthentication(builder.Configuration);

builder.Services.AddSwaggerGen(c => c.ConfigureSwaggerAuthentication());

var app = builder.Build();

await using (var scope = app.Services.CreateAsyncScope())
{
    await using var context = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();
    await context.Database.MigrateAsync();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(corsPolicyAlias);
app.UseAuthentication();
app.UseAuthorization();

app.MapPost("signin", async (string login, string password, AuthenticationService authenticationService) =>
{
    var result = await authenticationService.Authenticate(login, password);
    if(!result.IsSuccess) return Results.BadRequest();

    return Results.Ok(result.GetResult());
});

app.MapPost("signup", async (string login, string password, AuthenticationService authenticationService) =>
{
    var succeed = await authenticationService.RegisterUser(login, password);
    if (succeed) return Results.BadRequest();

    return Results.Ok();
});

app.Run();