using LAB.DataScanner.ConfigDatabaseApi.Contracts.Repositories;
using LAB.DataScanner.ConfigDatabaseApi.DataAccess.Entities;
using LAB.DataScanner.ConfigDatabaseApi.DataAccess.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace LAB.DataScanner.ConfigDatabaseApi.DataAccess.Registrators
{
    public static class DataAccessRegistrator
    {
        public static IServiceCollection AddDataAccess(this IServiceCollection services) => services
            .AddTransient<IBaseRepository<ApplicationInstance>, ApplicationInstancesRepository>()
            .AddTransient<IBaseRepository<ApplicationType>, ApplicationTypesRepository>()
            .AddTransient<IBaseRepository<Binding>, BindingsRepository>();
    }
}
