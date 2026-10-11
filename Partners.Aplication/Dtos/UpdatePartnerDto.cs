
using Partners.Domain;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Partners.Application;

public class UpdatePartnerDto
{
    private string _name = string.Empty;
    private string _taxNumber = string.Empty;
    private string? _email;

    [Required(ErrorMessage = "O campo Name é obrigatório.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "O Name deve ter entre 2 e 100 caracteres.")]
    public string Name
    {
        get => _name;
        set => _name = value?.Trim() ?? string.Empty;
    }

    [Required(ErrorMessage = "O TaxNumber é obrigatório.")]
    [RegularExpression(@"^\d{9}$", ErrorMessage = "O TaxNumber deve conter exatamente 9 dígitos numéricos.")]
    public string TaxNumber
    {
        get => _taxNumber;
        set => _taxNumber = string.IsNullOrWhiteSpace(value)
            ? string.Empty
            : Regex.Replace(value.Trim(), @"[\s.-]", "");
    }

    [Required(ErrorMessage = "O tipo é obrigatório.")]
    [EnumDataType(typeof(PartnerType), ErrorMessage = "O tipo deve ser CUSTOMER, SUPPLIER ou BOTH.")]
    public PartnerType Type { get; set; }

    [EmailAddress(ErrorMessage = "O endereço de e-mail é inválido.")]
    [MaxLength(150)]
    public string? Email
    {
        get => _email;
        set => _email = string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
    }

    [Range(0, 1000000, ErrorMessage = "O CreditLimit deve estar entre 0 e 1.000.000.")]
    public decimal CreditLimit { get; set; }

    public bool IsActive { get; set; }
}
