using LAB.DataScanner.ConfigDatabaseApi.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LAB.DataScanner.ConfigDatabaseApi.DataAccess.EF.ModelConfigurations
{
    public class BindingConfiguration : IEntityTypeConfiguration<Binding>
    {
        public void Configure(EntityTypeBuilder<Binding> builder)
        {
            builder.HasNoKey();

            builder.ToTable("Binding", "binding");

            builder.Property(e => e.ConsumerInstanceId).HasColumnName("ConsumerInstanceID");

            builder.Property(e => e.PublisherInstanceId).HasColumnName("PublisherInstanceID");

            builder.HasOne(d => d.ConsumerInstance)
                .WithMany()
                .HasForeignKey(d => d.ConsumerInstanceId)
                .HasConstraintName("FK_CustomerInstance_Bindings");

            builder.HasOne(d => d.PublisherInstance)
                .WithMany()
                .HasForeignKey(d => d.PublisherInstanceId)
                .HasConstraintName("FK_AppInstance_Bindings");
        }
    }
}
