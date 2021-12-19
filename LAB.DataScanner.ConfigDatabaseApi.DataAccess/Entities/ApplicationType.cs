using System.Collections.Generic;

#nullable disable

namespace LAB.DataScanner.ConfigDatabaseApi.DataAccess.Entities
{
    public partial class ApplicationType
    {
        public ApplicationType()
        {
            ApplicationInstances = new HashSet<ApplicationInstance>();
        }

        public int TypeId { get; set; }
        public string TypeName { get; set; }
        public string TypeVersion { get; set; }
        public string ConfigTemplateJson { get; set; }
        public virtual ICollection<ApplicationInstance> ApplicationInstances { get; set; }
    }
}
