using System.ComponentModel.DataAnnotations;

namespace Api.Domain.DTO.User
{
    public class UserDTOCreate
    {
        [Required(ErrorMessage = "Nome e obrigatorio")]
        [StringLength(60, ErrorMessage = "Nome deve ter no maximo {1} caracteres")]
        public string Name { get; set; }
        
        [Required(ErrorMessage = "E-mail é obigatorio")]
        [EmailAddress(ErrorMessage = "Formato inválido de email")]
        [StringLength(100, ErrorMessage = "Email deve ter no máximo {1} caracteres")]
        public string Email { get; set; }
    }
}
