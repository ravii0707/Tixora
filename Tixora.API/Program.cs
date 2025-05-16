using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using Tixora.Repository.Implementations;
using Tixora.Repository.Interfaces;
using Tixora.Service.Implementations;
using Tixora.Service.Interfaces;
using Tixora.Core.Context;
using Tixora.API.Middleware;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Tixora.Core.Constants;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Tixora.Service.Mpapping;
using Serilog;


var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("Logs/api-log.txt", rollingInterval: RollingInterval.Day)
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Host.UseSerilog(); 

#region Add Services to the container

// Controllers and JSON options
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

//  Authentication and JWT configuration
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});


// Database Context
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("TixoraConnection")));

// AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

// Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IMovieRepository, MovieRepository>();
builder.Services.AddScoped<IShowTimeRepository, ShowTimeRepository>();
builder.Services.AddScoped<IBookingRepository, BookingRepository>();

// Services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IMovieService, MovieService>();
builder.Services.AddScoped<IShowTimeService, ShowTimeService>();
builder.Services.AddScoped<IBookingService, BookingService>();

// Swagger with genre enum listing
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.MapType<string>(() => new OpenApiSchema
    {
        Type = "string",
        Enum = ValidGenres.Genres.OrderBy(g => g)
            .Select(g => new OpenApiString(g))
            .Cast<IOpenApiAny>()
            .ToList()
    });
});

// CORS for Angular frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("*", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

#endregion

//  Build the app (services are now locked and read-only)
var app = builder.Build();

#region Configure the HTTP request pipeline

// Use CORS policy
app.UseCors("*");

// Swagger only in development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Enable HTTPS redirection
app.UseHttpsRedirection();



// Custom Middlewares
app.UseMiddleware<RequestLoggingMiddleware>();
app.UseMiddleware<ExceptionMiddleware>();
app.UseMiddleware<AuthorizationMiddleware>();

// Use Authentication and Authorization
app.UseAuthentication();  //
app.UseAuthorization();

// Map API controllers
app.MapControllers();

app.Lifetime.ApplicationStopping.Register(() =>
{
    var summary = RequestLoggingMiddleware.GetSuccessRate();
    Log.Information("=== API SUCCESS SUMMARY ===\n{summary}", summary);
});

#endregion

// Run the application
app.Run();
