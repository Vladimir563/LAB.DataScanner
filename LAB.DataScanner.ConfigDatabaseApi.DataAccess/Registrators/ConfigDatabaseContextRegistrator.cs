using LAB.DataScanner.ConfigDatabaseApi.DataAccess.EF;
using Microsoft.Extensions.DependencyInjection;

namespace LAB.DataScanner.ConfigDatabaseApi.DataAccess.Registrators
{
    public static class ConfigDatabaseContextRegistrator
    {
        public static IServiceCollection AddConfigDbContext(this IServiceCollection services) => services
            .AddScoped<IConfigDatabaseContext, DataScannerDbContext>();
    }
}
