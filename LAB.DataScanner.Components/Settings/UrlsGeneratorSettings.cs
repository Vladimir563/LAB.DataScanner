using LAB.DataScanner.Components.Settings.SettingsLibrary;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LAB.DataScanner.Components.Settings
{
    public class UrlsGeneratorSettings : BaseSettings
    {
        #region Application
        [Required]
        public string UrlTemplate { get; set; }
        [Required]
        public List<int[]> Sequences { get; set; }
        #endregion
    }
}
