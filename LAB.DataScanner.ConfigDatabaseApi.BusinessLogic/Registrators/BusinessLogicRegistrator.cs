using LAB.DataScanner.ConfigDatabaseApi.BusinessLogic.Services;
using LAB.DataScanner.ConfigDatabaseApi.Contracts.Services;
using LAB.DataScanner.ConfigDatabaseApi.DataAccess.Entities;
using Microsoft.Extensions.DependencyInjection;

namespace LAB.DataScanner.ConfigDatabaseApi.BusinessLogic.Registrators
{
    public static class BusinessLogicRegistrator
    {
        public static IServiceCollection AddBusinessLogic(this IServiceCollection services) => services
        .AddTransient<IBaseService<ApplicationType>, ApplicationTypeService>()
        .AddTransient<IBaseService<ApplicationInstance>, ApplicationInstanceService>()
        .AddTransient<IBaseService<Binding>, BindingService>();
    }
}
