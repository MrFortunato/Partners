
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Partners.Domain;

namespace Partners.Infra.Data;

internal class PartnersSchemaBuilder : IEntityTypeConfiguration<Partner>
{
    public void Configure(EntityTypeBuilder<Partner> builder)
    {
        builder.ToTable("Partners", table =>
        {
            table.HasCheckConstraint(
                "CK_Partners_Name_Length",
                "LEN(LTRIM(RTRIM([Name]))) >= 2");

            table.HasCheckConstraint(
                "CK_Partners_TaxNumber_Format",
                "LEN([TaxNumber]) = 9 AND [TaxNumber] NOT LIKE '%[^0-9]%'");

            table.HasCheckConstraint(
                "CK_Partners_CreditLimit_Range",
                "[CreditLimit] >= 0 AND [CreditLimit] <= 1000000");
        });

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd()
            .IsRequired();

        builder.Property(x => x.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.TaxNumber)
            .HasMaxLength(9)
            .IsRequired();

        builder.HasIndex(x => x.TaxNumber)
            .IsUnique();

        builder.Property(x => x.Type)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.Email)
            .HasMaxLength(150)
            .IsRequired(false);

        builder.Property(x => x.CreditLimit)
            .HasColumnType("decimal(18,2)")
            .HasDefaultValue(0m)
            .IsRequired();

        builder.Property(x => x.IsActive)
            .HasDefaultValue(true)
            .IsRequired();

        // Manter como int se a aplicação gerir a versão.
        builder.Property(x => x.Version)
            .HasDefaultValue(1)
            .IsConcurrencyToken()
            .IsRequired();

        // Data gerida pelo SQL Server em UTC.
        builder.Property(x => x.CreatedAt)
            .HasColumnType("datetime2")
            .HasDefaultValueSql("SYSUTCDATETIME()")
            .IsRequired();

        builder.Property(x => x.UpdatedAt)
            .HasColumnType("datetime2")
            .IsRequired(false);
    }
}
