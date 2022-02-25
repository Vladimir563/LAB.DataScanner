using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Management.Automation;

namespace LAB.DataScanner.SFDeploymentTool
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .Build();

            var scriptsPath = Path.GetFullPath(Directory.GetCurrentDirectory() + @"\..\..\..\scripts\windows\");

            SFDeploymentSettings deploymentSettings = new SFDeploymentSettings();

            configuration.GetSection("Settings").Bind(deploymentSettings);

            var deployAppScriptsPath = scriptsPath + @"deployApplication\";

            var createServiceScriptsPath = scriptsPath + @"createService\";

            var removeAppScriptsPath = scriptsPath + @"removeApplication\";

            var removeServiceScriptsPath = scriptsPath + @"removeService\";

            #region DeployApplication
            AddApplicationToImageStore(deployAppScriptsPath, deploymentSettings.ApplicationPkgPath,
                deploymentSettings.ApplicationPkgNameInImgStore);

            RegisterApplication(deployAppScriptsPath, deploymentSettings.ApplicationPkgNameInImgStore);

            CreateApplicationType(deployAppScriptsPath, deploymentSettings.ApplicationNameInCluster,
                deploymentSettings.ApplicationExeFileName, deploymentSettings.ApplicationVersion);

            PowerShell.Create().AddScript($@"powershell -ExecutionPolicy Bypass {Path.Combine(deployAppScriptsPath, "addApplicationToImageStore.ps1")}")
                .AddScript($@"powershell -ExecutionPolicy Bypass  {Path.Combine(deployAppScriptsPath, "registerApplication.ps1")}")
                .AddScript($@"powershell -ExecutionPolicy Bypass  {Path.Combine(deployAppScriptsPath, "createApplicationType.ps1")}")
                .Invoke();
            #endregion

            while (true)
            {
                Console.Write("Enter the command ('addservice', 'delservice', 'delapp', 'unregapp' or 'q' to exit):");

                var command = Console.ReadLine();

                if (command.Equals("q"))
                {
                    break;
                }
                else if (command.Equals("delapp"))
                {
                    RemoveApplicationType(removeAppScriptsPath, deploymentSettings.ApplicationNameInCluster);
                    UnregisterApplicationType(removeAppScriptsPath, deploymentSettings.ApplicationExeFileName + "Type", deploymentSettings.ApplicationVersion);
                }
                else if (command.Equals("addservice"))
                {
                    Console.Write("Service name: ");

                    var serviceName = Console.ReadLine();

                    Console.Write("Service type: ");

                    var serviceType = Console.ReadLine();

                    AddService(createServiceScriptsPath, deploymentSettings.ApplicationNameInCluster, serviceName, serviceType);
                }
                else if (command.Equals("delservice"))
                {
                    Console.Write("Service name for remove: ");

                    var serviceName = Console.ReadLine();

                    RemoveService(removeServiceScriptsPath, deploymentSettings.ApplicationNameInCluster, serviceName);
                }
            }
        }

        public static void AddApplicationToImageStore(string scriptPath, string applicationPkgPath, string applicationPkgNameInImageStore, int timeoutSec = 1800) 
        {
            var script = "Connect-ServiceFabricCluster" + Environment.NewLine +
                        @$"$path = {applicationPkgPath}" + Environment.NewLine +
                        $"Copy-ServiceFabricApplicationPackage -ApplicationPackagePath " +
                        $"$path -ApplicationPackagePathInImageStore {applicationPkgNameInImageStore} -TimeoutSec {timeoutSec}";

            PowerShell.Create().AddCommand("New-Item")
                .AddParameter("Path", scriptPath)
                .AddParameter("Name", "addApplicationToImageStore.ps1")
                .AddParameter("ItemType", "file")
                .AddParameter("Value", script)
                .AddParameter("-Force")
                .Invoke();
        }

        public static void RegisterApplication(string scriptsPath, string applicationPkgNameInImageStore)
        {
            var script = "Connect-ServiceFabricCluster" + Environment.NewLine +
                        $"Register-ServiceFabricApplicationType -ApplicationPathInImageStore {applicationPkgNameInImageStore}";

            PowerShell.Create().AddCommand("New-Item")
               .AddParameter("Path", scriptsPath)
               .AddParameter("Name", "registerApplication.ps1")
               .AddParameter("ItemType", "file")
               .AddParameter("Value", script)
               .AddParameter("-Force")
               .Invoke();
        }

        public static void CreateApplicationType(string scriptsPath, string applicationName, string applicationExeFileName, string version) 
        {
            var script = "Connect-ServiceFabricCluster" + Environment.NewLine +
                        $"New-ServiceFabricApplication fabric:/{applicationName} {applicationExeFileName + "Type"} {version}";

            PowerShell.Create().AddCommand("New-Item")
               .AddParameter("Path", scriptsPath)
               .AddParameter("Name", "createApplicationType.ps1")
               .AddParameter("ItemType", "file")
               .AddParameter("Value", script)
               .AddParameter("-Force")
               .Invoke();
        }

        public static void RemoveApplicationType(string scriptsPath, string applicationNameInCluster) 
        {
            var script = "Connect-ServiceFabricCluster" + Environment.NewLine +
                        $"Remove-ServiceFabricApplication fabric:/{applicationNameInCluster} -Force";

            PowerShell.Create().AddCommand("New-Item")
               .AddParameter("Path", scriptsPath)
               .AddParameter("Name", "removeApplicationType.ps1")
               .AddParameter("ItemType", "file")
               .AddParameter("Value", script)
               .AddParameter("-Force")
               .Invoke();

            PowerShell.Create().AddScript($@"powershell -ExecutionPolicy Bypass  {Path.Combine(scriptsPath, "removeApplicationType.ps1")}")
                .Invoke();
        }

        public static void UnregisterApplicationType(string scriptsPath, string applicationTypeName, string applicationVersion) 
        {
            var script = "Connect-ServiceFabricCluster" + Environment.NewLine +
                        $"Unregister-ServiceFabricApplicationType {applicationTypeName} {applicationVersion} -Force";

            PowerShell.Create().AddCommand("New-Item")
               .AddParameter("Path", scriptsPath)
               .AddParameter("Name", "unregisterApplicationType.ps1")
               .AddParameter("ItemType", "file")
               .AddParameter("Value", script)
               .AddParameter("-Force")
               .Invoke();

            PowerShell.Create().AddScript($@"powershell -ExecutionPolicy Bypass  {Path.Combine(scriptsPath, "unregisterApplicationType.ps1")}")
                .Invoke();
        }

        public static void AddService(string scriptsPath, string appName, string serviceName, string serviceTypeName)
        {
            var script = "Connect-ServiceFabricCluster" + Environment.NewLine + 
                        $"New-ServiceFabricService -ApplicationName fabric:/{appName} " +
                        $"-ServiceName fabric:/{appName}/{serviceName} " +
                        $"-ServiceTypeName {serviceTypeName} " +
                        "-Stateless " +
                        "-PartitionSchemeSingleton " +
                        $"-InstanceCount -1";

            PowerShell.Create().AddCommand("New-Item")
               .AddParameter("Path", scriptsPath)
               .AddParameter("Name", "addService.ps1")
               .AddParameter("ItemType", "file")
               .AddParameter("Value", script)
               .AddParameter("-Force")
               .Invoke();

            PowerShell.Create().AddScript($@"powershell -ExecutionPolicy Bypass  {Path.Combine(scriptsPath, "addService.ps1")}")
                .Invoke();
        }

        public static void RemoveService(string scriptsPath, string appName, string serviceName)
        {
            var script = "Connect-ServiceFabricCluster" + Environment.NewLine +
                        $"Remove-ServiceFabricService -ServiceName fabric:/{appName}/{serviceName} -Force";

            PowerShell.Create().AddCommand("New-Item")
               .AddParameter("Path", scriptsPath)
               .AddParameter("Name", "removeService.ps1")
               .AddParameter("ItemType", "file")
               .AddParameter("Value", script)
               .AddParameter("-Force")
               .Invoke();

            PowerShell.Create().AddScript($@"powershell -ExecutionPolicy Bypass  {Path.Combine(scriptsPath, "removeService.ps1")}")
                .Invoke();
        }
    }
}
