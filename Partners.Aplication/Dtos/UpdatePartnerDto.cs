using System.ComponentModel.DataAnnotations;
using Partners.Domain;

namespace Partners.Application;

public class UpdatePartnerDto
{
    [Required(ErrorMessage = "O nome é obrigatório.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "O nome deve ter entre 2 e 100 caracteres.")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "O TaxNumber é obrigatório.")]
    [RegularExpression(@"^\d{9}$", ErrorMessage = "O TaxNumber deve conter exatamente 9 dígitos numéricos.")]
    public string TaxNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "O tipo é obrigatório.")]
    [EnumDataType(typeof(PartnerType), ErrorMessage = "O tipo deve ser CUSTOMER, SUPPLIER ou BOTH.")]
    public PartnerType Type { get; set; }

    [EmailAddress(ErrorMessage = "O email fornecido não é válido.")]
    [MaxLength(150, ErrorMessage = "O email não pode exceder 150 caracteres.")]
    public string? Email { get; set; }

    [Range(0, 1000000, ErrorMessage = "O limite de crédito deve estar entre 0 e 1.000.000.")]
    [RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "CreditLimit deve ter no máximo duas casas decimais.")]
    public decimal CreditLimit { get; set; }

    public bool IsActive { get; set; }

    [Required(ErrorMessage = "A versão do registo é obrigatória para atualizações.")]
    public int Version { get; set; }
}
