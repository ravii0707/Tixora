//using System.Text.Json.Serialization;
//using Tixora.Repository.Implementations;
//using Tixora.Repository.Interfaces;
//using Tixora.Service.Implementations;
//using Tixora.Service.Interfaces;

//var builder = WebApplication.CreateBuilder(args);

//// Add services to the container.
//builder.Services.AddControllers()
//    .AddJsonOptions(options =>
//    {
//        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
//        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
//    });
//// AutoMapper
//builder.Services.AddAutoMapper(typeof(Program));

//// Repositories
//builder.Services.AddScoped<IUserRepository, UserRepository>();
//builder.Services.AddScoped<IMovieRepository, MovieRepository>();
//builder.Services.AddScoped<IShowTimeRepository, ShowTimeRepository>();
//builder.Services.AddScoped<IBookingRepository, BookingRepository>();

//// Services
//builder.Services.AddScoped<IUserService, UserService>();
//builder.Services.AddScoped<IMovieService, MovieService>();
//builder.Services.AddScoped<IShowTimeService, ShowTimeService>();
//builder.Services.AddScoped<IBookingService, BookingService>();

//// Swagger
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

//builder.Services.AddControllers();
//// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

//var app = builder.Build();

//// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

//app.UseHttpsRedirection();

//app.UseAuthorization();

//app.MapControllers();

//app.Run();



using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using Tixora.Repository.Implementations;
using Tixora.Repository.Interfaces;
using Tixora.Service.Implementations;
using Tixora.Service.Interfaces;
using Tixora.Service;
using Tixora.Core.Context;

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
builder.Services.AddAutoMapper(typeof(MappingProfile)); // Reference your mapping profile class

// Repositories - example of correct registration
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IMovieRepository, MovieRepository>();
builder.Services.AddScoped<IShowTimeRepository, ShowTimeRepository>();
builder.Services.AddScoped<IBookingRepository, BookingRepository>();

// Services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IMovieService, MovieService>();
builder.Services.AddScoped<IShowTimeService, ShowTimeService>();
builder.Services.AddScoped<IBookingService, BookingService>();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();