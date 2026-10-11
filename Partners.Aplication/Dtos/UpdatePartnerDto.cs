
using Partners.Domain;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Partners.Application;

public class UpdatePartnerDto
{
    [Required(ErrorMessage = "O nome é obrigatório.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "O nome deve ter entre 2 e 100 caracteres.")]
    public string Name { get; set; }

    [Required(ErrorMessage = "O TaxNumber é obrigatório.")]
    public string TaxNumber { get; set; }

    public PartnerType Type { get; set; }

    [EmailAddress(ErrorMessage = "O email fornecido não é válido.")]
    public string Email { get; set; }

    [Range(0, 1000000, ErrorMessage = "O limite de crédito deve estar entre 0 e 1.000.000.")]
    public decimal CreditLimit { get; set; }

    public bool IsActive { get; set; }

    [Required(ErrorMessage = "A versão do registo é obrigatória para atualizações.")]
    public int Version
    {
        get; set;
    }
}