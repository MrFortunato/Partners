using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Partners.Domain;

namespace Partners.Application;

public class CreatePartnerDto
{
    [Required(ErrorMessage = "O campo Name é obrigatório.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "O Name deve ter entre 2 e 100 caracteres.")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "O número fiscal é obrigatório.")]
    [DefaultValue("123456789")]
    [RegularExpression(@"^[\d\s.\-]+$", ErrorMessage = "O número fiscal deve conter apenas dígitos, pontos, espaços ou hífens.")]
    public string TaxNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "O tipo de Entidade é obrigatório.")]
    [EnumDataType(typeof(PartnerType), ErrorMessage = "O tipo de Entidade deve ser CUSTOMER, SUPPLIER ou BOTH.")]
    public PartnerType Type { get; set; }

    [EmailAddress(ErrorMessage = "O email fornecido não é válido.")]
    [MaxLength(150, ErrorMessage = "O email não pode exceder 150 caracteres.")]
    public string? Email { get; set; }

    [Range(0, 1000000, ErrorMessage = "O limite de crédito deve estar entre 0 e 1.000.000.")]
    [RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "CreditLimit deve ter no máximo duas casas decimais.")]
    public decimal CreditLimit { get; set; }
}
