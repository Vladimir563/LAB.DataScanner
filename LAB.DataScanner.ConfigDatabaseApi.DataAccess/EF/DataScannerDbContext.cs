using Microsoft.EntityFrameworkCore;
using LAB.DataScanner.ConfigDatabaseApi.DataAccess.Entities;
using LAB.DataScanner.ConfigDatabaseApi.DataAccess.EF.ModelConfigurations;

#nullable disable

namespace LAB.DataScanner.ConfigDatabaseApi.DataAccess.EF
{
    public partial class DataScannerDbContext : DbContext, IConfigDatabaseContext
    {
        public virtual DbSet<ApplicationInstance> ApplicationInstances { get; set; }
        public virtual DbSet<ApplicationType> ApplicationTypes { get; set; }
        public virtual DbSet<Binding> Bindings { get; set; }

        public DataScannerDbContext() { }

        public DataScannerDbContext(DbContextOptions<DataScannerDbContext> options)
            : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //if (!optionsBuilder.IsConfigured)
            //{
            //    optionsBuilder.UseSqlServer("Data Source=DESKTOP-4CJMFNM\\SQLEXPRESS;Initial Catalog=data_scanner_db;Integrated Security=True;Connect Timeout=60;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
            //}
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.ApplyConfiguration(new ApplicationInstanceConfiguration());

            modelBuilder.ApplyConfiguration(new ApplicationTypeConfiguration());

            modelBuilder.ApplyConfiguration(new BindingConfiguration());

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
