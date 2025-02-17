using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using JoseAnchaluisaVillonApi.Helpers;
using JoseAnchaluisaVillonApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Configuración de autenticación con JWT
var key = Encoding.ASCII.GetBytes(builder.Configuration["Jwt:Key"]);
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddSingleton<JwtService>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<DatabaseHelper>();
builder.Services.AddSingleton<ILogger<DatabaseHelper>, Logger<DatabaseHelper>>();
builder.Services.AddSingleton<IDatabaseHelper, DatabaseHelper>();

var logType = builder.Configuration["LoggingLog:Type"]; // "File" o "Database"

if (logType == "Database")
{
    builder.Services.AddSingleton<ILoggerService, DatabaseLoggerService>();
}
else
{
    builder.Services.AddSingleton<ILoggerService, FileLoggerService>();
}

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.Run();
