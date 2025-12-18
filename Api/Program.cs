using Microsoft.EntityFrameworkCore;
using Api.Data;
using Api.Repositories.Interfaces;
using Api.Repositories;
using Api.Services.Interfaces;  
using Api.Services;           
using Api.Mappings;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRentalPropertyRepository, RentalPropertyRepository>();
builder.Services.AddScoped<IBookingRepository, BookingRepository>();

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IPropertyService, PropertyService>();
builder.Services.AddScoped<IBookingService, BookingService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();

app.Run();