using System.ComponentModel.DataAnnotations;
namespace WebApiGemini.Models
{
    public class User
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Il nome è obbligatorio")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Il nome deve essere di almeno 3 caratteri")]
        public string? Nome { get; set; }
        [Required(ErrorMessage = "Il cognome è obbligatorio")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Il nome deve essere di almeno 3 caratteri")]
        public string? Cognome { get; set; }
        [Required(ErrorMessage = "L'email è obbligatoria")]
        [EmailAddress(ErrorMessage = "Formato email non valido")]
        public string? Email { get; set; }
        [Required(ErrorMessage = "La password è obbligatoria")]
        [MinLength(8, ErrorMessage = "La password deve essere di almeno 8 caratteri")]
        public string? Password { get; set; } // In un'applicazione reale, non dovresti mai memorizzare le password in chiaro
        public DateTime DataRegistrazione { get; set; } = DateTime.Now;
        public bool IsAdmin { get; set; } = false;
    }
}
