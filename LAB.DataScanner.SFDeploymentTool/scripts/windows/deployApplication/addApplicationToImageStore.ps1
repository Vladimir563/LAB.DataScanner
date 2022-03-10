Connect-ServiceFabricCluster
$path = 'D:\VSProjects\DataScanner\LAB.DataScanner.ServiceFabric\pkg\Debug'
Copy-ServiceFabricApplicationPackage -ApplicationPackagePath $path -ApplicationPackagePathInImageStore DataScannerPkg -TimeoutSec 1800