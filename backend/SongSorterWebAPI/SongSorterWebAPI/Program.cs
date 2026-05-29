using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SongSorterWebAPI.Data;
using SongSorterWebAPI.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);


var jwtSecret = builder.Configuration["JwtSettings:SecretKey"];

// 2. Додаємо систему аутентифікації
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        // Налаштовуємо правила перевірки токена
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false, // Для локальної розробки не перевіряємо видавця
            ValidateAudience = false, // Для локальної розробки не перевіряємо отримувача
            ValidateLifetime = true,  // Токен має "протухати" з часом
            ValidateIssuerSigningKey = true, // Обов'язкова перевірка нашого підпису
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret!))
        };

        // 3. НАЙГОЛОВНІШЕ: Вчимо сервер шукати токен у кукі
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                // Якщо браузер надіслав куку з назвою "jwt_token"
                if (context.Request.Cookies.ContainsKey("jwt_token"))
                {
                    // Беремо токен з неї
                    context.Token = context.Request.Cookies["jwt_token"];
                }
                return Task.CompletedTask;
            }
        };
    });




// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDataProtection();

builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<ITokenProtectionService, TokenProtectionService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:5173") // Зверни увагу: без слеша в кінці!
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});


var app = builder.Build();

// 2. ВМИКАЄМО CORS (Критично важливо: викликати це ДО UseAuthorization)
app.UseCors("AllowReactApp");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
