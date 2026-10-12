namespace Partners.Domain
{
    public class Partner
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string? TaxNumber { get; set; }
        public string Type { get; set; }
        public string? Email { get; set; }
        public decimal CreditLimit { get; set; }
        public bool IsActive { get; set; } = true;
        public int Version { get; set; } = 1;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
