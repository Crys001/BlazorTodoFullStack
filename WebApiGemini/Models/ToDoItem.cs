using System.ComponentModel.DataAnnotations;
namespace WebApiGemini.Models;

public class TodoItem
{
    public int Id { get; set; }
    public string? Titolo { get; set; }
    public bool IsCompletato { get; set; }
    public DateTime DataCreazione { get; set; } = DateTime.Now;
    public int UserId { get; set; }
    public User? User { get; set; }
    public Category? Category { get; set; }
    public int? CategoryId { get; set; }
}