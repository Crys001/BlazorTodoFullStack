namespace WebApiGemini.Shared;
public class TodoItemDTO
{
    public int Id { get; set; }
    public string? Titolo { get; set; }
    public bool IsCompletato { get; set; }
    public int? IdCategoria { get; set; }
    public string? NomeCategoria { get; set; }
}
