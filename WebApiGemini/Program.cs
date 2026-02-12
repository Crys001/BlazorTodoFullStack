using Microsoft.EntityFrameworkCore;
using WebApiGemini.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using WebApiGemini.Endpoints; // Importante: dove vivono MapTodoRoutes e MapAuthRoutes
var builder = WebApplication.CreateBuilder(args);

// --- 1. CONFIGURAZIONE SERVIZI (Dependency Injection) ---

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddEndpointsApiExplorer();

// Configurazione Swagger con supporto JWT
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Todo API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Inserisci il token JWT così: Bearer {tuo_token}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            new string[] {}
        }
    });
});

// Configurazione Sicurezza (JWT)
var key = Encoding.ASCII.GetBytes("QuestaChiaveDeveEssereLunghissimaESegreta123!");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

builder.Services.AddAuthorization();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazor", policy =>
    {
        policy.WithOrigins("https://localhost:7113") // L'URL dove girerà Blazor
              .AllowAnyMethod()                      // Permetti GET, POST, PUT, DELETE
              .AllowAnyHeader();                     // Permetti gli Header (importante per il Token JWT)
    });
});

var app = builder.Build();

// --- 2. CONFIGURAZIONE PIPELINE (Middleware) ---

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowBlazor");
// Fondamentali per far funzionare i lucchetti!
app.UseAuthentication();
app.UseAuthorization();

// --- 3. REGISTRAZIONE ENDPOINT (Utilizzando i tuoi nuovi file) ---

app.MapAuthRoutes(); // Spostato nel file Endpoints/AuthEndpoints.cs
app.MapTodoRoutes(); // Spostato nel file Endpoints/TodoEndpoints.cs
app.MapCategoryRoutes(); // Spostato nel file Endpoints/CategoryEndpoints.cs

app.Run();