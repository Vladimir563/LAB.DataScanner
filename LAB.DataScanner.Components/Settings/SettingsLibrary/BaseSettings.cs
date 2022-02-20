using System.ComponentModel.DataAnnotations;

namespace LAB.DataScanner.Components.Settings.SettingsLibrary
{
    public class BaseSettings
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string HostName { get; set; }
        [Required]
        public int Port { get; set; }
        [Required]
        public string VirtualHost { get; set; }
    }
}
