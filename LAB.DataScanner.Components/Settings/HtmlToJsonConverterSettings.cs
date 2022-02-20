using LAB.DataScanner.Components.Settings.SettingsLibrary;
using System.ComponentModel.DataAnnotations;

namespace LAB.DataScanner.Components.Settings
{
    public class HtmlToJsonConverterSettings : BaseSettings
    {
        #region Application
        [Required]
        public string HtmlDataDownloadingMethod { get; set; }
        [Required]
        public string WebBrowser { get; set; }
        [Required]
        public string HtmlFragmentStrategy { get; set; }
        [Required]
        public string HtmlFragmentExpression { get; set; }
        #endregion


        #region HtmlDataDownloadingSettingsArrs
        [Required]
        public string[] HtmlDataDownloadingMethods { get; set; }
        [Required]
        public string[] WebBrowsers { get; set; }
        [Required]
        public string[] HtmlFragmentStrategies { get; set; }
        #endregion

        #region DBTableCreationSettings
        [Required]
        public string[] Colums { get; set; }
        #endregion
    }
}