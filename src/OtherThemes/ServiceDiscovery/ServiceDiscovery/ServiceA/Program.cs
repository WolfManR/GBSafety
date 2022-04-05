using Consul;
using ServiceA;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IConsulClient, ConsulClient>(p => new ConsulClient(c => c.Address = new Uri("http://127.0.0.1:8500")));
builder.Services.AddHealthChecks();
builder.Services.AddHostedService<ConsulServiceRegistration>();
builder.Services.AddSingleton<Random>();

var app = builder.Build();

app.MapHealthChecks("/healthz");

app.MapGet("rand", (Random random) => Results.Ok(random.Next(0, 450)));

app.Run();
