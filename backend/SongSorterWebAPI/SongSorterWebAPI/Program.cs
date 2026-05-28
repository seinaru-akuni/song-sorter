using Microsoft.EntityFrameworkCore;
using SongSorterWebAPI.Data;
using SongSorterWebAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// 1. ДОДАЄМО СЕРВІС CORS (Вказуємо порт нашого React-додатка)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReact", policy =>
    {
        policy.WithOrigins("http://localhost:5173") // Стандартний порт Vite
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDataProtection(); 
builder.Services.AddScoped<ITokenProtectionService, TokenProtectionService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:5173") // Зверни увагу: без слеша в кінці!
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});


var app = builder.Build();

// 2. ВМИКАЄМО CORS (Критично важливо: викликати це ДО UseAuthorization)
app.UseCors("AllowReact");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
