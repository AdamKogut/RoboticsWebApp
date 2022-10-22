using Backend.Interfaces.Repository;
using Backend.Interfaces.Services;
using Backend.Models;
using Backend.Repositories;
using Backend.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.IdentityModel.Tokens;
using System.Text;


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

//authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(o =>
{
  o.TokenValidationParameters = new TokenValidationParameters
  {
    ValidIssuer = builder.Configuration["Jwt:Issuer"],
    ValidAudience = builder.Configuration["Jwt:Audience"],
    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
    ValidateIssuer = true,
    ValidateAudience = true,
    ValidateIssuerSigningKey = true
  };
});

// User Management
builder.Services.AddSingleton<IUserManagementRepository, UserManagementRepository>();
builder.Services.AddSingleton<ITeamService, TeamService>();
builder.Services.AddSingleton<IUserService, UserService>();

IConfiguration configuration = builder.Configuration;
var password = configuration["sqldb"];

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(string.Format(builder.Configuration.GetConnectionString("dbConnection"), password)));

var app = builder.Build();

app.UseRouting();

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
  ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

app.UseForwardedHeaders();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

using (var serviceScope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
{
  using (var context = serviceScope.ServiceProvider.GetService<ApplicationDbContext>())
  {
    try
    {
      context!.Database.Migrate();
    }
    catch (Exception e)
    {
      app.Logger.LogError($"DB Migration failed. Error was:{e}");
    }
  }
}

app.Run();
