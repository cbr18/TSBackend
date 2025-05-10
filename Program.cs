using Microsoft.EntityFrameworkCore;
using TSBackend.Data;
using TSBackend.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using DotNetEnv;

Env.Load(".env");
string connetctionString = Env.GetString("DB_CONNECTION");
string jwtSecret = Env.GetString("JWT_SECRET") ?? "YourSuperSecretKey123!@#$ThisShouldBeAtLeast32BytesLong";
string jwtIssuer = Env.GetString("JWT_ISSUER") ?? "YourIssuer";
string jwtAudience = Env.GetString("JWT_AUDIENCE") ?? "YourAudience";

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.ConfigureKestrel(serverOptions => {
    serverOptions.ListenAnyIP(7234); // или конкретный порт, который вы используете
});

// Добавление сервисов в контейнер
builder.Services.AddControllers();

// Регистрация контекста БД с использованием PostgreSQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connetctionString));

// Настройка Identity
builder.Services.AddIdentity<User, IdentityRole<int>>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// Настройка JWT аутентификации
builder.Services.AddAuthentication(options => {
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options => {
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
        ClockSkew = TimeSpan.Zero
    };
});

// Настройка Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Настройка CORS (разрешить все для разработки)
builder.Services.AddCors(options => {
    options.AddPolicy("AllowAll", policy => {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Применение миграций БД при запуске
using (var scope = app.Services.CreateScope()) {
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

// Конфигурация пайплайна запросов
if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors("AllowAll");
}

app.UseHttpsRedirection();

// Изменил порядок: сначала аутентификация, потом авторизация
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();