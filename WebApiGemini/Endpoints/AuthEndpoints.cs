using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Win32;
using System.ComponentModel.DataAnnotations; // Necessario per la validazione
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApiGemini.Data;
using WebApiGemini.Models;
using WebApiGemini.Shared;


namespace WebApiGemini.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthRoutes(this IEndpointRouteBuilder app)
    {
        // --- 1. REGISTRAZIONE CON VALIDAZIONE ---
        app.MapPost("/register", async (RegisterDTO registro, AppDbContext db) =>
        {
            // 1. Controllo email esistente
            if (await db.Users.AnyAsync(u => u.Email == registro.Email))
                return Results.BadRequest("Email già esistente.");

            // 2. Mappatura completa dal DTO alla classe User
            var nuovoUtente = new User
            {
                Nome = registro.Nome,
                Cognome = registro.Cognome,
                Email = registro.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(registro.Password),
                DataRegistrazione = DateTime.Now,
                IsAdmin = false
            };

            db.Users.Add(nuovoUtente);
            await db.SaveChangesAsync();

            return Results.Ok(new { message = "Utente registrato con successo!" });
        });

        // --- 2. LOGIN ---
        app.MapPost("/login", async (LoginDTO loginData, AppDbContext db, IConfiguration configuration) =>
        {
            var user = await db.Users.FirstOrDefaultAsync(u => u.Email == loginData.Email);

            if (user != null && BCrypt.Net.BCrypt.Verify(loginData.Password, user.Password))
            {
                var jwtSettings = configuration.GetSection("Jwt");
                var key = Encoding.ASCII.GetBytes(jwtSettings["Key"] ?? "ChiaveDiBackupMoltoLungaPerSicurezza123!");

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[]
                    {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(ClaimTypes.Role, user.IsAdmin ? "Admin" : "User")
            }),
                    Issuer = jwtSettings["Issuer"],
                    Audience = jwtSettings["Audience"],
                    Expires = DateTime.UtcNow.AddHours(1),
                    SigningCredentials = new SigningCredentials(
                        new SymmetricSecurityKey(key),
                        SecurityAlgorithms.HmacSha256Signature)
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                var token = tokenHandler.CreateToken(tokenDescriptor);

                return Results.Ok(new { Token = tokenHandler.WriteToken(token) });
            }

            return Results.Unauthorized();
        });

        app.MapGet("/users", async (AppDbContext db) =>
        {
            var users = await db.Users
                .Select(u => new UserDTO
                {
                    Id = u.Id,
                    Nome = u.Nome,
                    Cognome = u.Cognome,
                    Email = u.Email,
                    IsAdmin = u.IsAdmin,
                    DataRegistrazione = u.DataRegistrazione
                })
                .ToListAsync();

            return Results.Ok(users);
        })
.RequireAuthorization(policy => policy.RequireRole("Admin"));
    }
}