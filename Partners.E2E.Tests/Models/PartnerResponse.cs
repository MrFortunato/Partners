using System.Text.Json.Serialization;

namespace Partners.E2E.Tests.Models;

/// <summary>Modelo de resposta que espelha o JSON devolvido pela API.</summary>
public class PartnerResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string TaxNumber { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string? Email { get; set; }
    public decimal CreditLimit { get; set; }
    public bool IsActive { get; set; }
    public int Version { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>Modelo do corpo de erro devolvido pelo middleware.</summary>
public class ErrorResponse
{
    public int Status { get; set; }
    public string Error { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}
