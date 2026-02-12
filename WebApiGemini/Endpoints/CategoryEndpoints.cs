using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebApiGemini.Data;
using WebApiGemini.Shared;
using WebApiGemini.Models;

namespace WebApiGemini.Endpoints
{
    public static class CategoryEndpoints
    {
        public static void MapCategoryRoutes(this IEndpointRouteBuilder app)
        {
            //categorie per utente, quindi con autenticazione obbligatoria
            var group = app.MapGroup("/categories").RequireAuthorization();
            group.MapGet("/", async (AppDbContext db, ClaimsPrincipal user) =>
            {
                var userIdString = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userIdString == null) return Results.Unauthorized();

                int userId = int.Parse(userIdString);
                var myCategories = await db.Categories
                        .Where(x => x.UserId == userId)
                        .Select(x => new CategoryDTO
                        {
                            Id = x.Id,
                            Name = x.Name,
                        })
                        .ToListAsync();
                return Results.Ok(myCategories);
            });
            // Creazione di una nuova categoria per utente 
            group.MapPost("/", async (CategoryDTO category, ClaimsPrincipal user, AppDbContext db) =>
            {
                // 1. Estraiamo l'ID dell'utente dal Token (è una stringa, la convertiamo in int)
                var userIdString = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (userIdString == null) return Results.Unauthorized();

                int userId = int.Parse(userIdString);

                // 2. Creiamo l'oggetto reale collegandolo all'utente
                var newCategory = new Category
                {
                    Name = category.Name,
                    UserId = userId, // <-- Ecco il collegamento!
                };

                db.Categories.Add(newCategory);
                await db.SaveChangesAsync();

                return Results.Created($"/categories/{newCategory.Id}", newCategory);
            });

            // --- DELETE: ELIMINA CATEGORIA (E OPZIONALMENTE SCOLLEGA I TODO) ---
            group.MapDelete("/{id}", async (int id, AppDbContext db, ClaimsPrincipal user) =>
            {
                var userIdString = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userIdString == null) return Results.Unauthorized();
                int userId = int.Parse(userIdString);

                // Cerchiamo la categoria verificando che appartenga all'utente loggato
                var category = await db.Categories
                    .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);

                if (category == null)
                {
                    // Se entriamo qui, l'API restituisce 404 ed è quello che vedi in console
                    return Results.NotFound("Categoria non trovata o non autorizzato.");
                }

                // Scolleghiamo i Todo associati per evitare errori di vincolo nel DB
                var todos = await db.TodoItems.Where(t => t.CategoryId == id).ToListAsync();
                foreach (var todo in todos)
                {
                    todo.CategoryId = null;
                }

                db.Categories.Remove(category);
                await db.SaveChangesAsync();

                return Results.NoContent(); // 204 Successo
            });
        }
    }
}
