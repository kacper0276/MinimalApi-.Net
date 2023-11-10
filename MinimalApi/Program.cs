using Microsoft.AspNetCore.Mvc;
using MinimalApi.ToDo;
using FluentValidation;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Service
builder.Services.AddSingleton<IToDoService, ToDoService>();

// Service FluentValidator
builder.Services.AddValidatorsFromAssemblyContaining(typeof(ToDoValidator));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// ToDoRequests.RegisterEndpoints(app);
app.RegisterEndpoints();

app.Run();