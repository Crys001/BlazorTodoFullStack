using System.ComponentModel.DataAnnotations;

namespace WebApiGemini.Shared;
public class TodoItemDTO
{
    public int Id { get; set; }
    [Required(ErrorMessage = "Il titolo è obbligatorio")]
    [StringLength(100, ErrorMessage = "Il titolo è troppo lungo")]
    public string? Titolo { get; set; }
    public bool IsCompletato { get; set; }
    public int? IdCategoria { get; set; }
    public string? NomeCategoria { get; set; }
}
