using LAB.DataScanner.ConfigDatabaseApi.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LAB.DataScanner.ConfigDatabaseApi.DataAccess.EF.ModelConfigurations
{
    public class ApplicationTypeConfiguration : IEntityTypeConfiguration<ApplicationType>
    {
        public void Configure(EntityTypeBuilder<ApplicationType> builder)
        {
            builder.HasKey(e => e.TypeId)
                .HasName("PK__Applicat__516F03956D3AA083");

            builder.ToTable("ApplicationType", "meta");

            builder.Property(e => e.TypeId).HasColumnName("TypeID");

            builder.Property(e => e.TypeName).HasMaxLength(50);

            builder.Property(e => e.TypeVersion).HasMaxLength(12);
        }
    }
}
