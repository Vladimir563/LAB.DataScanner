using LAB.DataScanner.Components.Settings.SettingsLibrary;
using System.ComponentModel.DataAnnotations;

namespace LAB.DataScanner.Components.Settings
{
    public class SimpleTableDBPersisterSettings : BaseSettings
    {
        #region DBTableCreationSettings
        [Required]
        public string SqlConnectionString { get; set; }
        [Required]
        public string Dbo { get; set; }
        [Required]
        public string Schema { get; set; }
        [Required]
        public string TableName { get; set; }
        [Required]
        public string OwnerName { get; set; }
        [Required]
        public string[] Colums { get; set; }
        #endregion
    }
}