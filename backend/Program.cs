using Backend.Interfaces;
using Backend.Models;
using Backend.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add configuration
builder.Host.ConfigureAppConfiguration((hostingContext, config) =>
{
  config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
  config.AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true);

  if (Environments.Production == Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"))
  {
    config.AddKeyPerFile(directoryPath: "/secrets", optional: false);
  }
  else
  {
    var parentDir = Path.GetFullPath($"..");
    config.AddKeyPerFile(directoryPath: $"{parentDir}/secrets", optional: false);
  }
});

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IUserManagementRepository, UserManagementRepository>();

IConfiguration configuration = builder.Configuration;
var password = configuration["sqldb"];

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(string.Format(builder.Configuration.GetConnectionString("dbConnection"), password)));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
