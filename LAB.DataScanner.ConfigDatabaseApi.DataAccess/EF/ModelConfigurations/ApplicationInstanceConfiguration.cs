using LAB.DataScanner.ConfigDatabaseApi.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LAB.DataScanner.ConfigDatabaseApi.DataAccess.EF.ModelConfigurations
{
    public class ApplicationInstanceConfiguration : IEntityTypeConfiguration<ApplicationInstance>
    {
        public void Configure(EntityTypeBuilder<ApplicationInstance> builder)
        {
            builder.HasKey(e => e.InstanceId)
                .HasName("PK__Applicat__5C51996FDD8C7C2F");

            builder.ToTable("ApplicationInstance", "component");

            builder.Property(e => e.InstanceId).HasColumnName("InstanceID");

            builder.Property(e => e.InstanceName).HasMaxLength(50);

            builder.Property(e => e.TypeId).HasColumnName("TypeID");

            builder.HasOne(d => d.Type)
                .WithMany(p => p.ApplicationInstances)
                .HasForeignKey(d => d.TypeId)
                .HasConstraintName("FK_AppType_AppInstance");
        }
    }
}
