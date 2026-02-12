using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebApiGemini.Data;
using WebApiGemini.Shared;
using WebApiGemini.Models;

namespace WebApiGemini.Endpoints
{
    public static class TodoEndpoints
    {
        public static void MapTodoRoutes(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/todoitems").RequireAuthorization();

            // --- GET: RECUPERA TODO DELL'UTENTE (MODIFICATO) ---
            group.MapGet("/", async (AppDbContext db, ClaimsPrincipal user) =>
            {
                var userIdString = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userIdString == null) return Results.Unauthorized();

                int userId = int.Parse(userIdString);

                var myTodos = await db.TodoItems
                        .Include(x => x.Category) // <--- FONDAMENTALE: Carica la relazione con la tabella categorie
                        .Where(x => x.UserId == userId)
                        .Select(x => new TodoItemDTO
                        {
                            Id = x.Id,
                            Titolo = x.Titolo,
                            IsCompletato = x.IsCompletato,
                            IdCategoria = x.CategoryId,
                            // Se la categoria esiste, passiamo il nome, altrimenti "Senza Categoria"
                            NomeCategoria = x.Category != null ? x.Category.Name : "Senza Categoria"
                        })
                        .ToListAsync();

                return Results.Ok(myTodos);
            }).RequireAuthorization();

            // --- PUT: AGGIORNA TODO ---
            group.MapPut("/{id}", async (int id, TodoItemDTO updatedItem, AppDbContext db, ClaimsPrincipal user) =>
            {
                var userIdString = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userIdString == null) return Results.Unauthorized();
                int userId = int.Parse(userIdString);

                var item = await db.TodoItems.FindAsync(id);
                if (item == null) return Results.NotFound();

                if (item.UserId != userId) return Results.Forbid();

                item.Titolo = updatedItem.Titolo;
                item.IsCompletato = updatedItem.IsCompletato;
                item.CategoryId = updatedItem.IdCategoria;

                await db.SaveChangesAsync();
                return Results.NoContent();
            });

            // --- DELETE: ELIMINA TODO ---
            group.MapDelete("/{id}", async (int id, AppDbContext db, ClaimsPrincipal user) =>
            {
                var userIdString = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userIdString == null) return Results.Unauthorized();
                int userId = int.Parse(userIdString);

                var item = await db.TodoItems.FindAsync(id);
                if (item == null) return Results.NotFound();

                if (item.UserId != userId) return Results.Forbid();

                db.TodoItems.Remove(item);
                await db.SaveChangesAsync();
                return Results.NoContent();
            });

            // --- POST: CREA NUOVO TODO ---
            group.MapPost("/", async (TodoItemDTO dto, ClaimsPrincipal user, AppDbContext db) =>
            {
                var userIdString = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userIdString == null) return Results.Unauthorized();
                int userId = int.Parse(userIdString);

                // Controllo se la categoria appartiene all'utente
                if (dto.IdCategoria.HasValue)
                {
                    var categoryExists = await db.Categories.AnyAsync(c => c.Id == dto.IdCategoria.Value && c.UserId == userId);
                    if (!categoryExists) return Results.BadRequest("La categoria non esiste o non appartiene all'utente.");
                }

                var newTodo = new TodoItem
                {
                    Titolo = dto.Titolo,
                    IsCompletato = dto.IsCompletato,
                    UserId = userId,
                    DataCreazione = DateTime.Now,
                    CategoryId = dto.IdCategoria
                };

                db.TodoItems.Add(newTodo);
                await db.SaveChangesAsync();

                return Results.Created($"/todoitems/{newTodo.Id}", newTodo);
            });

            // --- GET ADMIN: TUTTI I TODO ---
            group.MapGet("/admin/all", async (AppDbContext db) =>
            {
                var allTodos = await db.TodoItems
                    .Include(t => t.User)
                    .Include(t => t.Category) // Includiamo anche qui per completezza
                    .Select(x => new
                    {
                        x.Id,
                        x.Titolo,
                        x.IsCompletato,
                        x.DataCreazione,
                        Categoria = x.Category != null ? x.Category.Name : "Nessuna",
                        Autore = x.User != null ? x.User.Email : "Sconosciuto"
                    })
                    .ToListAsync();

                return Results.Ok(allTodos);
            }).RequireAuthorization(policy => policy.RequireRole("Admin"));
        }
    }
}