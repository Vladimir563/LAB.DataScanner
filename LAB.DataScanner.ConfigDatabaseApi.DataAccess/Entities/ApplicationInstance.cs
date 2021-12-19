#nullable disable

namespace LAB.DataScanner.ConfigDatabaseApi.DataAccess.Entities
{
    public partial class ApplicationInstance
    {
        public int InstanceId { get; set; }
        public int? TypeId { get; set; }
        public string InstanceName { get; set; }
        public string ConfigJson { get; set; }
        public virtual ApplicationType Type { get; set; }
    }
}
