using LAB.DataScanner.Components.Interfaces.Downloaders;
using LAB.DataScanner.Components.Services.MessageBroker.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client.Events;
using System.Collections.Concurrent;
using System.Text;
using System.Threading.Tasks;

namespace LAB.DataScanner.Components.Services.Downloaders
{
    public class WebPageDownloaderEngine : IDownloaderEngine
    {
        private readonly IConfigurationRoot _configuration;

        private readonly IDataRetriever _dataRetriever;

        private readonly IRmqPublisher _rmqPublisher;

        private readonly IRmqConsumer _rmqConsumer;

        private readonly string[] _routingKeys;

        private readonly string _exchangeName;

        //TODO: As for me, it looks like a static methods of helper
        private readonly IUrlsValidator _urlsValidator;

        private readonly ILogger<WebPageDownloaderEngine> _logger;

        public WebPageDownloaderEngine(IConfigurationRoot configuration,
            IDataRetriever dataRetriever, IRmqPublisher rmqPublisher, 
            IRmqConsumer rmqConsumer, IUrlsValidator urlsValidator, ILogger<WebPageDownloaderEngine> logger)
        {
            _configuration = configuration;

            _dataRetriever = dataRetriever;

            _rmqPublisher = rmqPublisher;

            _rmqConsumer = rmqConsumer;

            _urlsValidator = urlsValidator;

            _logger = logger;

            _routingKeys = JsonConvert.DeserializeObject<string[]>(_configuration
                .GetSection("Binding:SenderRoutingKeys").Value ?? "");

            _exchangeName = _configuration.GetSection("Binding:SenderExchange").Value ?? "";
        }

        public void StartEngine()
        {
            //TODO: Does it suit to add try catch clause?
            _logger.LogInformation("DownloaderEngine has been started");

            _rmqConsumer.StartListening(OnReceive);
        }

        //TODO: What is the reason to have a public method? I remember you  had a question regarding the unit tests. Please remind me during the meeting.
        public void OnReceive(object model, BasicDeliverEventArgs ea) 
        {
            var body = ea.Body.ToArray();

            var url = Encoding.UTF8.GetString(body);

            _rmqConsumer.Ack(ea);

            Task.Run(async () =>
            {
                await ProcessUrlAsync(url);
            });
        }

        private async Task ProcessUrlAsync(string url)
        {
            if (!_urlsValidator.IsUrlValid(url))
            {
                _logger.LogError($"@Url: {url} is not valid");
                return;
            }

            var data = await _dataRetriever.RetrieveStringAsync(url);

            var byteArrHtmlContent = Encoding.UTF8.GetBytes(data);

            _rmqPublisher.Publish(byteArrHtmlContent, _exchangeName, _routingKeys);
        }
    }
}
