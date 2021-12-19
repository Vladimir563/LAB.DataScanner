//using LAB.DataScanner.ConfigDatabaseApi.DataAccess.Repositories;
//using LAB.DataScanner.ConfigDatabaseApi.DataAccess.RepositoryContracts;
using Microsoft.Extensions.DependencyInjection;

namespace LAB.DataScanner.ConfigDatabaseApi.DataAccess.Registrators
{
    public static class DataAccessRegistrator
    {
        public static IServiceCollection AddDataAccess(this IServiceCollection services) => services;
        //.AddScoped<IApplicationInstancesRepository, ApplicationInstancesRepository>()
        //.AddScoped<IApplicationTypesRepository, ApplicationTypesRepository>()
        //.AddScoped<IBindingsRepository, BindingsRepository>();
    }
}
