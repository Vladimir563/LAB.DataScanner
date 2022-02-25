
namespace LAB.DataScanner.SFDeploymentTool
{
    public class SFDeploymentSettings
    {
        public string ApplicationPkgPath { get; set; }
        public string ApplicationPkgNameInImgStore { get; set; }
        public string ApplicationNameInCluster { get; set; }
        public string ApplicationExeFileName { get; set; }
        public string ApplicationVersion { get; set; }
        public string ClusterUri { get; set; }
    }
}