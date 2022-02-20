using Microsoft.Extensions.Configuration;
using System.Fabric;

namespace LAB.DataScanner.Components
{
    public static class ServiceFabricConfigurationExtensions
    {
        public static IConfigurationBuilder AddJsonFile(this IConfigurationBuilder builder, ServiceContext serviceContext)
        {
            // get the Config package directory
            var configFolderPath = serviceContext.CodePackageActivationContext.GetConfigurationPackageObject("Config").Path;

            var appSettingsFilePath = System.IO.Path.Combine(configFolderPath, "appsettings.json");

            builder.AddJsonFile(appSettingsFilePath, optional: false, reloadOnChange: true);

            return builder;
        }
    }
}
