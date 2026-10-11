
using System.ComponentModel.DataAnnotations;

namespace Partners.Application;

public class DeletePartnerDto
{
    [Required(ErrorMessage = "O Id do entidade é obrigatório.")]
    public Guid Id { get; set; }
}
