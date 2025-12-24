using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Api.Data;
using Api.Repositories.Interfaces;
using Api.Repositories;
using Api.Services.Interfaces;  
using Api.Services;           
using Api.Mappings;
using Api.Middleware;

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

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Настройка JSON сериализации: camelCase для имен свойств
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.WriteIndented = true;
    })
    .ConfigureApiBehaviorOptions(options =>
    {
        // Отключаем автоматическую обработку ошибок валидации, чтобы контроллеры сами обрабатывали их
        options.SuppressModelStateInvalidFilter = true;
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Система аренды жилья API",
        Version = "v1",
        Description = "API для управления системой аренды жилья"
    });
});

var app = builder.Build();

// Регистрация middleware обработки исключений (должен быть первым в конвейере)
app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();

app.Run();