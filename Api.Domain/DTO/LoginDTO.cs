using System.ComponentModel.DataAnnotations;

namespace Api.Domain.DTO
{
    public class LoginDTO
    {
        [Required(ErrorMessage = "Necessário informar o E-mail para Login")]
        [EmailAddress(ErrorMessage = "Formato inválido de email")]
        [StringLength(100, ErrorMessage = "Email deve ter no máximo {1} caracteres")]
        public string Email { get; set; }
    }
}
