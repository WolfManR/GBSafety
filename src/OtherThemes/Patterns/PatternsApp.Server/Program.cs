using Microsoft.AspNetCore.ResponseCompression;
using PatternsApp.Server.Hubs;
using PatternsApp.Server.Services;
using PatternsApp.Shared;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSignalR();
//builder.Services.AddResponseCompression(o => o.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] { "application/octet-stream" }));
builder.Services.AddCors(option => {
    option.AddPolicy("cors", policy => {
        policy.AllowAnyOrigin().AllowAnyHeader();
    });
});

builder.Services.AddSingleton<ChatsStorage>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("cors");

app.MapHub<ChatHub>("/chathub");

app.Run();