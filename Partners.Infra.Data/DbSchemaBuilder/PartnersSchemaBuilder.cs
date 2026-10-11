using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Partners.Domain;

namespace Partners.Infra.Data.DbSchemaBuilder
{
    internal class PartnersSchemaBuilder : IEntityTypeConfiguration<Partner>
    {

        public void Configure(EntityTypeBuilder<Partner> builder)
        {
            // Nome da tabela (boa prática especificar explicitamente)
            builder.ToTable("Partners");

            builder.HasKey(x => x.Id);

            // Propriedades da tabela
            builder.Property(x => x.Id)
                .ValueGeneratedOnAdd()
                .IsRequired();
            // Nota: Se 'Id' for Guid, o EF Core mapeia nativamente para 'UNIQUEIDENTIFIER' no SQL Server. 
            // Remova o HasColumnType("BINARY(16)"), a menos que esteja guardando o Guid como binário de forma customizada.

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
