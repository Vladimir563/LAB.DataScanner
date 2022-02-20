using System.ComponentModel.DataAnnotations;

namespace LAB.DataScanner.Components.Settings
{
    public class RmqConsumerSettings
    {
        [Required]
        public string ReceiverQueue { get; set; }
        [Required]
        public string ReceiverExchange { get; set; }
        [Required]
        public string ReceiverRoutingKey { get; set; }
        [Required]
        public string ExchangeType { get; set; }
    }
}