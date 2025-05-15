using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using Tixora.Repository.Implementations;
using Tixora.Repository.Interfaces;
using Tixora.Service.Implementations;
using Tixora.Service.Interfaces;
using Tixora.Service;
using Tixora.Core.Context;
using Tixora.API.Middleware;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });



// Database Context
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("TixoraConnection")));

// AutoMapper - this is the correct way to register it
builder.Services.AddAutoMapper(typeof(MappingProfile));




// Repositories - example of correct registration
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IMovieRepository, MovieRepository>();
builder.Services.AddScoped<IShowTimeRepository, ShowTimeRepository>();
builder.Services.AddScoped<IBookingRepository, BookingRepository>();
builder.Services.AddScoped<IMovieRepository, MovieRepository>();
builder.Services.AddScoped<IShowTimeRepository, ShowTimeRepository>();

// Services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IMovieService, MovieService>();
builder.Services.AddScoped<IShowTimeService, ShowTimeService>();
builder.Services.AddScoped<IBookingService, BookingService>();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("*",
        policy =>
        {
            policy.WithOrigins("http://localhost:4200") 
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});
var app = builder.Build();
app.UseCors("*");

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
// Add the exception handling middleware
app.UseMiddleware<ExceptionMiddleware>();
app.UseAuthorization();
app.MapControllers();

app.Run();