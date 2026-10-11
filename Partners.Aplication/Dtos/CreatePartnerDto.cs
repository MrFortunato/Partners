using Partners.Domain;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;

namespace Partners.Aplication
{
    public class CreatePartnerDto
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(150)]
        private string _name = string.Empty;

        [Required(ErrorMessage = "O campo Name é obrigatorio.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "O Name deve ter entre 2 e 100 caracteres.")]
        public string Name
        {
            get => _name;
            set => _name = value?.Trim() ?? string.Empty;
        }

        private string _taxNumber = string.Empty;
        [Required]
        [RegularExpression(@"^\d{9}$", ErrorMessage = "O TaxNumber deve conter exactamente 9 dígitos numericos.")]
        public string TaxNumber
        {
            get => _taxNumber;
            set => _taxNumber = NormalizeTaxNumber(value);
        }

        [Required(ErrorMessage = "O tipo é obrigatório.")]
        [EnumDataType(typeof(PartnerType), ErrorMessage = "O tipo deve ser Cliente, Fornecedor ou Ambos.")]
        public PartnerType Type { get; set; }

        private string? _email;
        [EmailAddress]
        [MaxLength(150)]
        public string? Email
        {
            get => _email;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    _email = null;
                }
                else
                {
                    var trimmed = value.Trim();
                    _email = string.IsNullOrEmpty(trimmed) ? null : trimmed;
                }
            }
        }


        [Range(0, 1000000)]
        [RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "CreditLimit deve ter no máximo duas casas decimais.")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal CreditLimit { get; set; }

        public bool IsActive { get; set; } = true;

        public int Version { get; set; } = 1;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public static string NormalizeTaxNumber(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;


            var cleaned = Regex.Replace(input, @"[\s\.\-]", "");

            if (!Regex.IsMatch(cleaned, @"^\d{9}$"))
            {
                throw new ArgumentException("O TaxNumber normalizado deve conter exatamente 9 dígitos numéricos.");
            }

            return cleaned;
        }
    }
}
