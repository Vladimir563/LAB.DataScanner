using LAB.DataScanner.ConfigDatabaseApi.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace LAB.DataScanner.ConfigDatabaseApi.DataAccess.EF
{
    public interface IConfigDatabaseContext
    {
        DbSet<ApplicationInstance> ApplicationInstances { get; set; }
        DbSet<ApplicationType> ApplicationTypes { get; set; }
        DbSet<Binding> Bindings { get; set; }
    }
}
