namespace WebApiGemini.Shared;

public class UserDTO
{
    public int Id { get; set; }
    public string? Nome { get; set; }
    public string? Cognome { get; set; }
    public string? Email { get; set; }
    public bool IsAdmin { get; set; }
    public DateTime DataRegistrazione { get; set; }
}