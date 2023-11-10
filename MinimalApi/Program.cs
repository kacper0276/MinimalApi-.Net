using Microsoft.AspNetCore.Mvc;
using MinimalApi.ToDo;
using FluentValidation;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Service
builder.Services.AddSingleton<IToDoService, ToDoService>();

// Service FluentValidator
builder.Services.AddValidatorsFromAssemblyContaining(typeof(ToDoValidator));

// JWT Implementacja
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer(cfg =>
    {
        cfg.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidIssuer = builder.Configuration["JwtIssuer"],
            ValidAudience = builder.Configuration["JwtIssuer"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtKey"]))
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// ToDoRequests.RegisterEndpoints(app);
app.RegisterEndpoints();

app.MapGet("/token", () =>
{
    var claims = new[]
    {
        new Claim(ClaimTypes.NameIdentifier, "user-id"),
        new Claim(ClaimTypes.Name, "Test name"),
        new Claim(ClaimTypes.Role, "Admin")
    };

    var token = new JwtSecurityToken
        (
            issuer: builder.Configuration["JwtIssuer"],
            audience: builder.Configuration["JwtIssuer"],
            claims: claims,
            expires: DateTime.UtcNow.AddDays(60),
            notBefore: DateTime.UtcNow,
            signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtKey"])),
                    SecurityAlgorithms.HmacSha256)
        );

    var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);
    return jwtToken;
});

// Info o zalogowanym uzytkowniku
app.MapGet("/hello", (ClaimsPrincipal user) =>
{
    var userName = user.Identity.Name;
    return $"Hello {userName}";
});

app.Run();