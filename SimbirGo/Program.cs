using System.Data.Common;
using System.Reflection;
using FluentMigrator.Runner;
using Microsoft.IdentityModel.Tokens;
using Npgsql;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using SimbirGo.Models.Base;
using SimbirGo.Repositories;
using SimbirGo.Repositories.Interfaces;
using SimbirGo.Services;
using SimbirGo.Services.Interfaces;
using SimbirGo.Settings;
using SimbirGo.Settings.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Adding JWT authentication
builder.Services.AddAuthorization();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>    
    {
        options.ClaimsIssuer = AuthOptions.Issuer;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            // указывает, будет ли валидироваться издатель при валидации токена
            ValidateIssuer = true,
            // строка, представляющая издателя
            ValidIssuer = AuthOptions.Issuer,
            // будет ли валидироваться потребитель токена
            ValidateAudience = true,
            // установка потребителя токена
            ValidAudience = AuthOptions.Audience,
            // будет ли валидироваться время существования
            ValidateLifetime = true,
            // установка ключа безопасности
            IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),
            // валидация ключа безопасности
            ValidateIssuerSigningKey = true,
        };
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                {
                    context.Response.Headers.Add("Token-Expired", "true");
                }

                return Task.CompletedTask;
            }
        };
    });

// Add services to the container
builder.Services.AddSingleton<IConfigurationSettings, ConfigurationSettings>();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IJwtService, JwtService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Connecting to the database
builder.Services.AddTransient<DbConnection>(s =>
    new NpgsqlConnection("Server=localhost:5432;Database=simbirGoDb;User Id=postgres;Password=admin;"));

// Setup migrations
builder.Services
    .AddFluentMigratorCore().ConfigureRunner(rb =>
        rb.AddPostgres()
            .WithGlobalConnectionString("Server=localhost:5432;Database=simbirGoDb;User Id=postgres;Password=admin;")
            .ScanIn(Assembly.GetExecutingAssembly()).For.Migrations())
    .AddLogging(lb => lb.AddFluentMigratorConsole())
    .BuildServiceProvider(false);

// Running migrations
var app = builder.Build();
using var serviceProvider = app.Services.CreateScope();
var services = serviceProvider.ServiceProvider;
var runner = services.GetRequiredService<IMigrationRunner>();
runner.MigrateUp();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Adding authentication and authorization to the request pipeline
app.UseAuthentication();
app.UseAuthorization();

app.UseHttpsRedirection();
app.MapControllers();
app.Run();