using LAB.DataScanner.Components.Interfaces.Downloaders;
using LAB.DataScanner.Components.Services.MessageBroker.Interfaces;
using LAB.DataScanner.Components.Settings;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading.Tasks;

namespace LAB.DataScanner.Components.Services.Downloaders
{
    public class WebPageDownloaderEngine : IDownloaderEngine
    {
        private readonly RmqPublisherSettings _publisherSettings;

        private readonly IDataRetriever _dataRetriever;

        private readonly IRmqPublisher _rmqPublisher;

        private readonly IRmqConsumer _rmqConsumer;

        public WebPageDownloaderEngine(RmqPublisherSettings publisherSettings,
            IDataRetriever dataRetriever, IRmqPublisher rmqPublisher, 
            IRmqConsumer rmqConsumer)
        {
            _dataRetriever = dataRetriever;

            _rmqPublisher = rmqPublisher;

            _rmqConsumer = rmqConsumer;

            _publisherSettings = publisherSettings;
        }

        public void Start()
        {
            _rmqConsumer.StartListening(OnReceive);
        }

        private void OnReceive(object model, BasicDeliverEventArgs ea) 
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
            if (!IsUrlValid(url))
            {
                throw new ArgumentException($"@Url: {url} is not valid");
            }

            var data = await _dataRetriever.RetrieveStringAsync(url);

            if (data is null || data.Equals(string.Empty)) throw new ArgumentNullException($"The content from url ({url}) was null");

            var byteArrHtmlContent = Encoding.UTF8.GetBytes(data);

            _rmqPublisher.Publish(byteArrHtmlContent, _publisherSettings.SenderExchange, _publisherSettings.SenderRoutingKeys);
        }

        private bool IsUrlValid(string uriString) => Uri.TryCreate(uriString, UriKind.RelativeOrAbsolute, out Uri uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
    }
}
