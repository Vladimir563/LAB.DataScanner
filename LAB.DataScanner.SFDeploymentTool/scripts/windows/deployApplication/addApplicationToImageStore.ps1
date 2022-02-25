Connect-ServiceFabricCluster
$path = 'F:\VS2022_source\repos\LAB.DataScanner\LAB.DataScanner.ServiceFabric\pkg\Debug'
Copy-ServiceFabricApplicationPackage -ApplicationPackagePath $path -ApplicationPackagePathInImageStore DataScannerPkg -TimeoutSec 1800