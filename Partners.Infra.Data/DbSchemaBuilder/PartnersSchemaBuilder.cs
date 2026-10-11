using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Partners.Domain;

namespace Partners.Infra.Data
{
    internal class PartnersSchemaBuilder : IEntityTypeConfiguration<Partner>
    {

        public void Configure(EntityTypeBuilder<Partner> builder)
        {
      
            builder.ToTable("Partners");

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

            builder.Property(x => x.CreditLimit)
                 .HasColumnType("decimal(18,2)")
                 .IsRequired();

            builder.Property(x => x.IsActive)
                .HasDefaultValue(true)
                .IsRequired();

            builder.Property(x => x.Version)
                .IsRowVersion();

            // Datas gerenciadas pelo servidor em UTC (SYSUTCDATETIME)
            builder.Property(x => x.CreatedAt)
                .HasColumnType("datetime2")
                .HasDefaultValueSql("SYSUTCDATETIME()")
                .IsRequired();

            builder.Property(x => x.UpdatedAt)
                .HasColumnType("datetime2");


        }
    }
}
