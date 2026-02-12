using System.ComponentModel.DataAnnotations;

public class RegisterDTO
{
    [Required] public string Nome { get; set; } = "";
    [Required] public string Cognome { get; set; } = "";
    [Required, EmailAddress] public string Email { get; set; } = "";
    [Required, MinLength(8)] public string Password { get; set; } = "";
}
