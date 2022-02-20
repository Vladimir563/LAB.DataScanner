using System.ComponentModel.DataAnnotations;

namespace LAB.DataScanner.Components.Settings
{
    public class RmqPublisherSettings
    {
        [Required]
        public string SenderExchange { get; set; }
        [Required]
        public string BasicSenderRoutingKey { get; set; }
        [Required]
        public string[] SenderRoutingKeys { get; set; }
        [Required]
        public string ExchangeType { get; set; }
    }
}
