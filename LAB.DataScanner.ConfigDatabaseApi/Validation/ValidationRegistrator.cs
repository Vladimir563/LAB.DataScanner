using FluentValidation;
using LAB.DataScanner.ConfigDatabaseApi.DataAccess.Entities;
using Microsoft.Extensions.DependencyInjection;

namespace LAB.DataScanner.ConfigDatabaseApi.Validation
{
    public static class ValidationRegistrator
    {
        public static IServiceCollection AddValidation(this IServiceCollection services) => services
            .AddTransient<IValidator<ApplicationInstance>, ApplicationInstanceValidator>()
            .AddTransient<IValidator<ApplicationType>, ApplicationTypeValidator>()
            .AddTransient<IValidator<Binding>, BindingValidator>();

    }
}
