using LAB.DataScanner.Components.Services.MessageBroker.Interfaces;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Text;

namespace LAB.DataScanner.Components.Services.Downloaders
{
    public class WebPageDownloaderEngine
    {
        private readonly IConfigurationSection _bindingConfiguration;

        private readonly IDataRetriever _dataRetriever;

        private readonly IRmqPublisher _rmqPublisher;

        private readonly IRmqConsumer _rmqConsumer;


        public WebPageDownloaderEngine(IConfigurationSection bindingConfiguration,
            IDataRetriever dataRetriever, IRmqPublisher rmqPublisher, IRmqConsumer rmqConsumer)
        {
            _bindingConfiguration = bindingConfiguration;

            _dataRetriever = dataRetriever;

            _rmqPublisher = rmqPublisher;

            _rmqConsumer = rmqConsumer;
        }


        public IRmqConsumer GetConsumer() => _rmqConsumer;

        public IRmqPublisher GetPublisher() => _rmqPublisher;

        public void Start()
        {
            var _routingKeys = JsonConvert.DeserializeObject<string[]>(_bindingConfiguration
                .GetSection("SenderRoutingKeys").Value ?? "");

            var _exchangeName = _bindingConfiguration.GetSection("SenderExchange").Value;

            _rmqConsumer.StartListening(async (model, ea) =>
            {
                var body = ea.Body.ToArray();

                var url = Encoding.UTF8.GetString(body);

                var routingKey = ea.RoutingKey;

                _rmqConsumer.Ack(ea);

                Console.WriteLine(" [x] Received '{0}':'{1}'",
                                  routingKey,
                                  url);

                if (UrlIsValid(url))
                {
                    //get content by url
                    string pageDataAsString = await _dataRetriever.RetrieveStringAsync(url);

                    //sent to rabbitmq content which we got by url
                    byte[] content = Encoding.UTF8.GetBytes(pageDataAsString);

                    _rmqPublisher.Publish(content, _exchangeName, _routingKeys);
                }
            });
        }

        private bool UrlIsValid(string uriString)
        {
            return Uri.TryCreate(uriString, UriKind.RelativeOrAbsolute, out Uri uriResult)
            && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }
    }
}
